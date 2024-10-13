using System.Collections;
using System.Collections.Generic;
namespace Kurisu.NGDS
{
    /// <summary>
    /// Base class for modules in dialogue system
    /// </summary>
    public interface IDialogueModule
    {

    }
    /// <summary>
    /// Apply data directly after module added
    /// </summary>
    public interface IApplyable
    {
        void Apply(Node parentNode);
    }
    /// <summary>
    /// Process data after inject dependency
    /// </summary>
    public interface IProcessable
    {
        IEnumerator Process(IObjectResolver resolver);
    }
    /// <summary>
    /// Object dependency resolver
    /// </summary>
    public interface IObjectResolver
    {
        T Resolve<T>();
    }
    /// <summary>
    /// Base class for module contains dialogue content
    /// </summary>
    public interface IContentModule
    {
        void GetContents(List<string> contents);
        void AddContent(string content);
        void SetContents(List<string> contents);
    }
}
