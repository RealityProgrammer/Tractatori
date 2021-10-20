using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ManipulationUtilities
{
    // By design, each non-abstract node need an private, non-static Evaluate() method to be able to evaluated
    // Therefore, Reflection need to be used for both dynamically and ease-of-use
    internal static readonly string EvaluateMethodName = "Evaluate";

    public class NodeEvaluateCache {
        public MethodInfo Method { get; private set; }
        public ParameterInfo[] Parameters { get; private set; }
        public int OutParameterCount { get; private set; }
        public int ContainerParameterIndex { get; private set; } = -1;
        public int[] LayoutIndex { get; private set; }

        private static readonly HashSet<int> _duplicationBuffer = new HashSet<int>();

        public NodeEvaluateCache(MethodInfo method) {
            Method = method;
            Parameters = Method.GetParameters();
            OutParameterCount = Parameters.CountOutParameters();
            LayoutIndex = new int[Parameters.Length];

            if (Parameters.Length != 0) {
                if (Parameters[0].ParameterType == NodeEvaluationInfoType) {
                    ContainerParameterIndex = 0;
                } else {
                    for (int i = 0; i < Parameters.Length; i++) {
                        var parameter = Parameters[i];

                        if (parameter.ParameterType == NodeEvaluationInfoType && parameter.GetCustomAttribute<ExplicitContainerParameterAttribute>() != null) {
                            ContainerParameterIndex = i;
                            break;
                        }
                    }
                }

                int outTracker = 0;
                for (int i = 0; i < Parameters.Length; i++) {
                    var parameter = Parameters[i];

                    if (parameter.IsOut) {
                        var attr = parameter.GetCustomAttribute<OutputLayoutIndexAttribute>();

                        if (attr != null) {
                            LayoutIndex[i] = attr.Index;
                        } else {
                            LayoutIndex[i] = outTracker;
                            outTracker++;
                        }
                    } else {
                        LayoutIndex[i] = -1;
                    }
                }

                // Make sure there are no duplication
#if UNITY_EDITOR
                bool duplicated = false;

                _duplicationBuffer.Clear();
                foreach (var index in LayoutIndex) {
                    if (index < 0) continue;

                    if (_duplicationBuffer.Contains(index)) {
                        duplicated = true;
                        break;
                    } else {
                        _duplicationBuffer.Add(index);
                    }
                }

                if (duplicated) {
                    Debug.LogWarning("Evaluate method of node type " + Method.DeclaringType.FullName + " contains invalid output layout index. Thus the output layout index of this node will fallback to incremental sequence.");

                    int t = 0;
                    for (int i = 0; i < LayoutIndex.Length; i++) {
                        if (Parameters[i].IsOut) {
                            LayoutIndex[i] = t;
                            t++;
                        }
                    }
                }
#endif
            }
        }

        public bool UseContainerParameter => ContainerParameterIndex != -1;
        public ParameterInfo ContainerParameter => Parameters[ContainerParameterIndex];

        public override string ToString() {
            return $"NodeEvaluateCache(Method = Evaluate, Parameters = ({string.Join(", ", Parameters.Select(x => x.IsOut ? x.ParameterType.GetElementType().FullName + " (Out)" : x.ParameterType.FullName))}), Out Parameter Count = {OutParameterCount}, Container Parameter Index = {ContainerParameterIndex})";
        }
    }

    public static readonly Type VoidType = typeof(void);
    public static readonly Type YieldInstructionType = typeof(YieldInstruction);
    public static readonly Type CustomYieldInstructionType = typeof(CustomYieldInstruction);

    private static readonly Dictionary<Type, NodeEvaluateCache> _nodeEvaluateMethodCache = new Dictionary<Type, NodeEvaluateCache>();

    public static NodeEvaluateCache GetEvaluateCache<T>() where T : BaseRuntimeNode {
        return GetEvaluateCache(typeof(T));
    }

    private static readonly Type BaseRuntimeNodeType = typeof(BaseRuntimeNode);
    public static NodeEvaluateCache GetEvaluateCache(Type nodeType) {
        if (!nodeType.IsSubclassOf(BaseRuntimeNodeType)) {
            Debug.Log("Type " + nodeType.FullName + " is not a child class of BaseRuntimeNode.");
            return null;
        }

        if (nodeType.IsAbstract) {
            Debug.Log("Node type " + nodeType.FullName + " is marked as abstract. Cannot evaluate be evaluated.");
            return null;
        }

        return GetEvaluateCacheInternal(nodeType);
    }

    private static readonly Type NodeEvaluationInfoType = typeof(NodeEvaluationInfo);
    private static NodeEvaluateCache GetEvaluateCacheInternal(Type nodeType) {
        if (_nodeEvaluateMethodCache.TryGetValue(nodeType, out var output)) {
            return output;
        } else {
            var methods = nodeType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var method in methods) {
                if (method.Name == EvaluateMethodName) {
                    var parameters = method.GetParameters();

                    if (parameters.Length == 0 || (parameters.Length == 1 && parameters[0].ParameterType == NodeEvaluationInfoType)) {
                        output = new NodeEvaluateCache(method);
                        _nodeEvaluateMethodCache.Add(nodeType, output);

                        return output;
                    }
                }
            }

            return null;
        }
    }

    private static readonly object[] EmptyReturnArray = new object[0];
    public static object[] InvokeNodeEvaluateMethod(this BaseRuntimeNode node) {
        var cache = GetEvaluateCacheInternal(node.NodeType);

        if (cache != null) {
            var invokeParameters = new object[cache.Parameters.Length];

            int outSize = cache.OutParameterCount;

            cache.Method.Invoke(node, invokeParameters);

            if (outSize == 0) {
                return EmptyReturnArray;
            }

            return CollectOutReturns(invokeParameters, cache.Parameters, outSize);
        } else {
            return null;
        }
    }

    public static object[] InvokeNodeEvaluateMethod(this BaseRuntimeNode node, NodeEvaluationInfo container, out object methodReturn) {
        var cache = GetEvaluateCacheInternal(node.NodeType);

        if (cache != null) {
            var invokeParameters = new object[cache.Parameters.Length];
            
            if (cache.UseContainerParameter) {
                invokeParameters[cache.ContainerParameterIndex] = container;
            }

            int outSize = cache.OutParameterCount;

            methodReturn = cache.Method.Invoke(node, invokeParameters);

            if (outSize == 0) {
                return EmptyReturnArray;
            }

            return CollectOutReturns(invokeParameters, cache.Parameters, outSize);
        } else {
            methodReturn = null;
            return null;
        }
    }

    private static int CountOutParameters(this ParameterInfo[] infos) {
        int c = 0;
        foreach (var pi in infos) {
            if (pi.IsOut) {
                c++;
            }
        }

        return c;
    }

    private static object[] CollectOutReturns(object[] objectParameters, ParameterInfo[] parameterInfos, int outSize) {
        var ret = new object[outSize];

        int outTracker = 0;
        for (int i = 0; i < parameterInfos.Length; i++) {
            if (parameterInfos[i].IsOut) {
                ret[outTracker] = objectParameters[i];
                outTracker++;
            }

            if (outTracker == outSize) break;
        }

        return ret;
    }

    public static int FindParameterOutputIndex(Type nodeType, int searchIndex) {
        if (!nodeType.IsSubclassOf(BaseRuntimeNodeType)) {
            Debug.Log("Type " + nodeType.FullName + " is not a child class of BaseRuntimeNode.");
            return -1;
        }

        if (nodeType.IsAbstract) {
            Debug.Log("Node type " + nodeType.FullName + " is marked as abstract.");
            return -1;
        }

        var cache = GetEvaluateCacheInternal(nodeType);

        if (cache != null) {
            for (int i = 0; i < cache.LayoutIndex.Length; i++) {
                if (cache.LayoutIndex[i] == searchIndex) {
                    return i;
                }
            }
        }

        return -1;
    }

    public static int FindParameterOutputIndex<T>(int searchIndex) where T : BaseRuntimeNode {
        return FindParameterOutputIndex(typeof(T), searchIndex);
    }

    public static int FindParameterOutputIndexOutOnly(Type nodeType, int searchIndex) {
        if (!nodeType.IsSubclassOf(BaseRuntimeNodeType)) {
            Debug.Log("Type " + nodeType.FullName + " is not a child class of BaseRuntimeNode.");
            return -1;
        }

        if (nodeType.IsAbstract) {
            Debug.Log("Node type " + nodeType.FullName + " is marked as abstract.");
            return -1;
        }

        var cache = GetEvaluateCacheInternal(nodeType);

        if (cache != null) {
            int t = 0;

            for (int i = 0; i < cache.Parameters.Length; i++) {
                if (!cache.Parameters[i].IsOut) continue;

                if (cache.LayoutIndex[i] == searchIndex) {
                    return t;
                }

                t++;
            }
        }

        return -1;
    }

    public static int FindParameterOutputIndexOutOnly<T>(int searchIndex) where T : BaseRuntimeNode {
        return FindParameterOutputIndexOutOnly(typeof(T), searchIndex);
    }
}
