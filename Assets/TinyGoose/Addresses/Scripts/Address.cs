// -----------------------------------------------------------------------------------------------------------------------------
//		TinyGoose Addresses
//      (c) 2023 Tiny Goose
// -----------------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TinyGoose.Addresses
{
    // -----------------------------------------------------------------------------------------------------------------------------
    //		Address - don't use this class directly
    // -----------------------------------------------------------------------------------------------------------------------------
    public class Address : IKeyEvaluator
    {
        protected Address(string rawAddress)
        {
            m_RawAddress = rawAddress;
        }
        
        internal readonly string m_RawAddress;
        
        // -----------------------------------------------------------------------------------------------------------------------------
        //		IKeyEvaluator
        // -----------------------------------------------------------------------------------------------------------------------------
        public bool RuntimeKeyIsValid() => !String.IsNullOrEmpty(m_RawAddress);
        public object RuntimeKey => m_RawAddress;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------------
    //		Address<T> class - generic Address
    // -----------------------------------------------------------------------------------------------------------------------------
    public class Address<T> : Address where T : UnityEngine.Object
    {
        public Address(string rawAddress) : base(rawAddress) { }
    }

    // -----------------------------------------------------------------------------------------------------------------------------
    //		SceneAddress class - for scenes
    // -----------------------------------------------------------------------------------------------------------------------------
    public class SceneAddress : Address
    {
        public SceneAddress(string rawAddress) : base(rawAddress) {}
    }
}