using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

[InitializeOnLoad]
public static class TypeCollector
{
    public static Dictionary<Assembly, List<Type>> CandidateTypeContainer { get; private set; }

    static TypeCollector() {
        CompilationPipeline.assemblyCompilationFinished += (string path, CompilerMessage[] msg) => {
            // Debug.Log("Finish Compilation. Output path: " + path + ". Compiler Message Length: " + msg.Length);

            var asm = CompilationPipeline.GetAssemblies().FirstOrDefault(x => x.outputPath == path);

            if ((asm.flags & AssemblyFlags.EditorAssembly) != AssemblyFlags.EditorAssembly) {
                // CandidateTypeContainer.Add(asm, asm.)
            }
        };
    }
}
