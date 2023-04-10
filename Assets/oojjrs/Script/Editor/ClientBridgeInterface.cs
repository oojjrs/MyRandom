using System;
using UnityEngine;

namespace Assets.oojjrs.Script.Editor
{
    public interface ClientBridgeInterface
    {
        bool Running { get; }

        T AcquireObject<T>() where T : class;
        T GetDebugEnum<T>(string name, T defValue) where T : Enum;
        bool GetDebugFlag(string name) => GetDebugFlag(name, default);
        bool GetDebugFlag(string name, bool defValue);
        Vector3 GetVector3(string name);
        void Send(object packet);
        void SetDebugEnum<T>(string name, T value) where T : Enum;
        void SetDebugFlag(string name, bool value);
        void SetVector3(string name, Vector3 value);
    }
}
