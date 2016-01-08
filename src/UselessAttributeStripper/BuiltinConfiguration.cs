namespace UselessAttributeStripper
{
    internal class BuiltinConfiguration
    {
        public static readonly string[] AttributeNames =
        {
            // Just information
            "System.Runtime.CompilerServices.CompilerGeneratedAttribute",
            "System.Runtime.CompilerServices.ExtensionAttribute",
            "System.ParamArrayAttribute",
            "System.Reflection.DefaultMemberAttribute",
            "System.Diagnostics.DebuggerStepThroughAttribute",
            "System.Diagnostics.DebuggerHiddenAttribute",
            "System.Diagnostics.DebuggerDisplayAttribute",
            "System.Diagnostics.CodeAnalysis.SuppressMessageAttribute",
            "System.ObsoleteAttribute",
            "System.AttributeUsageAttribute",
            "System.MonoTODOAttribute",
            // Not relative
            "System.CLSCompliantAttribute",
            "System.Runtime.InteropServices.ComVisibleAttribute",
            "System.Runtime.ConstrainedExecution.ReliabilityContractAttribute",
            // Editor only
            "UnityEngine.AddComponentMenu",
            "UnityEditor.MenuItem",
            "UnityEngine.ContextMenu",
            "UnityEngine.ExecuteInEditMode",
            "UnityEngine.HideInInspector",
            "UnityEngine.TooltipAttribute",
            "UnityEngine.DisallowMultipleComponent",
            "UnityEngine.Internal.ExcludeFromDocsAttribute",
        };
    }
}
