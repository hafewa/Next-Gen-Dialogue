using System;
using Ceres;
using Ceres.Annotations;
using Kurisu.NGDS;
using UnityEngine;
using UnityEngine.Localization;
namespace Kurisu.NGDT.Localization
{
    [Serializable]
    [NodeInfo("Module : Localized Content Module is used to modify dialogue content such as piece and option using Unity.Localization.")]
    [ModuleOf(typeof(Piece))]
    [ModuleOf(typeof(Option))]
    public class LocalizedContentModule : CustomModule
    {
        [SerializeField, Setting]
        private SharedString tableEntry;
        
        [SerializeField, Setting]
        private SharedString stringEntry;

        protected sealed override IDialogueModule GetModule()
        {
            return new NGDS.ContentModule(new LocalizedString(tableEntry.Value, stringEntry.Value).GetLocalizedString());
        }
    }
}
