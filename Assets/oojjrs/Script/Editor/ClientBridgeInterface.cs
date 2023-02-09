using System;

namespace Assets.Sources.Scripts.Editor
{
    public interface ClientBridgeInterface
    {
        bool Running { get; }

        T AcquireObject<T>() where T : class;
        T GetDebugEnum<T>(string name, T defValue) where T : Enum;
        bool GetDebugFlag(string name) => GetDebugFlag(name, default);
        bool GetDebugFlag(string name, bool defValue);
        void Send(object packet);
        void SetDebugEnum<T>(string name, T value) where T : Enum;
        void SetDebugFlag(string name, bool value);
    }
}
