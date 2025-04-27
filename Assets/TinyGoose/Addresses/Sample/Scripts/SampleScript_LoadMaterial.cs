using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MyGame
{
    // -----------------------------------------------------------------------------------------------------------------------------
    //		A super simple test class.
    //
    //      Loads a material via Unity Addressables, using Addresses to locate
    //      the asset.
    // -----------------------------------------------------------------------------------------------------------------------------
    public class SampleScript_LoadMaterial : MonoBehaviour
    {
        // A handle to our Addressable asset.
        // (this is a Unity Addressables class, not from Addresses)
        private AsyncOperationHandle<Material> m_MaterialHandle;

#region Not needed for your game - only for Sample :(
        [Conditional("UNITY_EDITOR")] void Awake() => EditorAddressables.SetUp();
#endregion

        private IEnumerator Start()
        {
            // Begin to load the material, using our Address
            m_MaterialHandle = Addressables.LoadAssetAsync<Material>(Addresses.M_Sample);
            
            // ... you can try other addresses e.g. Addresses.OtherCoolGroup.M_Sample_InOtherGroup
            
            // Yield to wait for Addressables system to fully load our material
            yield return m_MaterialHandle; 

            // Apply material to our renderer!
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>(); // don't use GetComponent in a real project ;)
            meshRenderer.material = m_MaterialHandle.Result;
        }

        private void OnDestroy()
        {
            // Cleanup addressable ref where relevant
            Addressables.Release(m_MaterialHandle);
        }
    }
}