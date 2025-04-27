// -----------------------------------------------------------------------------------------------------------------------------
//		TinyGoose Addresses
//      (c) 2023 Tiny Goose
// -----------------------------------------------------------------------------------------------------------------------------

using UnityEngine;

namespace TinyGoose.Addresses
{
    public enum AddressesFileExtension
    {
        Cs,
        GenCs
    }
    
    // -----------------------------------------------------------------------------------------------------------------------------
    //		AddressesAsset class
    // -----------------------------------------------------------------------------------------------------------------------------
    [CreateAssetMenu(menuName = "Addresses/Addresses Asset", fileName = "Addresses")]
    public class AddressesAsset : ScriptableObject
    {
        // -----------------------------------------------------------------------------------------------------------------------------
        //		Serialised
        // -----------------------------------------------------------------------------------------------------------------------------
        [Tooltip("Where to store the code-behind file")]
        [SerializeField] private string m_CodePath = "Assets/Code";
        
        [Tooltip("The namespace to use for the code-behind file")]
        [SerializeField] private string m_Namespace;
        
        [Tooltip("The class name to use for the code-behind file")]
        [SerializeField] private string m_ClassName = "Addresses";

        [Tooltip("The file extension to use (.gen.cs/.cs)")]
        [SerializeField] private AddressesFileExtension m_Extension;

        // -----------------------------------------------------------------------------------------------------------------------------
        //		Properties
        // -----------------------------------------------------------------------------------------------------------------------------
        public string CodePath => m_CodePath;
        public string Namespace => m_Namespace;
        public string ClassName => m_ClassName;
        public AddressesFileExtension Extension => m_Extension;
    }
}