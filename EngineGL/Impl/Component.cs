using System;
using EngineGL.Core;
using EngineGL.Event.LifeCycle;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace EngineGL.Impl
{
    public class Component : IComponent
    {
        public event EventHandler<InitialzeEventArgs> Initialze;
        public event EventHandler<UpdateEventArgs> Update;
        public event EventHandler<DestroyEventArgs> Destroy;

        private IComponentAttachable _parentObject;

        [JsonIgnore, YamlIgnore]
        public virtual IComponentAttachable ParentObject
        {
            get => _parentObject;
            set
            {
                if (_parentObject == null)
                    _parentObject = value;
                else
                    throw new InvalidOperationException("Component already has parent object linked.");
            }
        }

        [JsonIgnore, YamlIgnore]
        public virtual IGameObject GameObject
        {
            get => (IGameObject) _parentObject;
        }

        public Guid InstanceGuid { get; set; } = Guid.NewGuid();

        public virtual void OnInitialze()
        {
            Initialze?.Invoke(this, new InitialzeEventArgs(this));
        }

        public virtual void OnUpdate(double deltaTime)
        {
            Update?.Invoke(this, new UpdateEventArgs(this, deltaTime));
        }

        public virtual void OnDestroy()
        {
            Destroy?.Invoke(this, new DestroyEventArgs(this));
        }
    }
}