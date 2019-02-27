using System;
using System.Collections.Concurrent;
using System.IO;
using EngineGL.Core;
using EngineGL.Core.LifeCycle;
using EngineGL.Event.Game;
using EngineGL.Event.LifeCycle;
using EngineGL.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EngineGL.Impl
{
    public class Game : GameWindow, IGame
    {
        public string Name { get; set; }
        public ConcurrentDictionary<int, IScene> PreLoadedScenes { get; } = new ConcurrentDictionary<int, IScene>();
        public ConcurrentDictionary<int, IScene> LoadedScenes { get; } = new ConcurrentDictionary<int, IScene>();
        public event EventHandler<InitialzeEventArgs> Initialze;
        public event EventHandler<DestroyEventArgs> Destroy;
        public event EventHandler<LoadSceneEventArgs> LoadSceneEvent;
        public event EventHandler<UnloadSceneEventArgs> UnloadSceneEvent;
        public event EventHandler<PreLoadSceneEventArgs> PreLoadSceneEvent;
        public event EventHandler<PreUnloadSceneEventArgs> PreUnloadSceneEvent;

        public virtual void OnInitialze()
        {
            Initialze?.Invoke(this, new InitialzeEventArgs(this));
        }

        public virtual void OnDestroy()
        {
            Destroy?.Invoke(this, new DestroyEventArgs(this));
        }

        public Result<int> PreLoadScene(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            return PreLoadScene(fileInfo);
        }

        public Result<int> PreLoadScene(FileInfo file)
        {
            IScene scene = new Scene(file);
            int hash = scene.GetHashCode();

            PreLoadSceneEventArgs args = new PreLoadSceneEventArgs(this, file, scene);
            EventManager<PreLoadSceneEventArgs> manager
                = new EventManager<PreLoadSceneEventArgs>(PreLoadSceneEvent, this, args);
            manager.OnSuccess = ev => PreLoadedScenes.TryAdd(ev.PreLoadScene.GetHashCode(), ev.PreLoadScene);

            if (manager.Call())
                return Result<int>.Success(args.PreLoadScene.GetHashCode());
            else
                return Result<int>.Fail();
        }

        public bool PreUnloadScene(int hash)
        {
            if (PreLoadedScenes.TryGetValue(hash, out IScene scene))
            {
                PreUnloadSceneEventArgs args = new PreUnloadSceneEventArgs(this, scene);
                EventManager<PreUnloadSceneEventArgs> manager
                    = new EventManager<PreUnloadSceneEventArgs>(PreUnloadSceneEvent, this, args);
                manager.OnSuccess = ev => PreLoadedScenes.TryRemove(ev.PreUnloadScene.GetHashCode(), out scene);
                return manager.Call();
            }

            return false;
        }

        public bool PreUnloadScenes()
        {
            int c = 0;
            foreach (int hash in PreLoadedScenes.Keys)
            {
                if (PreUnloadScene(hash))
                    c++;
            }

            return c > 0;
        }

        public Result<IScene> GetScene(int hash)
        {
            if (LoadedScenes.TryGetValue(hash, out IScene scene))
            {
                return Result<IScene>.Success(scene);
            }

            return Result<IScene>.Fail();
        }

        public Result<T> GetSceneUnsafe<T>(int hash) where T : IScene
        {
            if (LoadedScenes.TryGetValue(hash, out IScene scene))
            {
                return Result<T>.Success((T) scene);
            }

            return Result<T>.Fail();
        }

        public Result<IScene> LoadScene(int hash)
        {
            if (PreLoadedScenes.TryGetValue(hash, out IScene scene)
                || !LoadedScenes.ContainsKey(hash))
            {
                LoadSceneEventArgs args = new LoadSceneEventArgs(this, scene);
                EventManager<LoadSceneEventArgs> manager
                    = new EventManager<LoadSceneEventArgs>(LoadSceneEvent, this, args);
                manager.OnSuccess = ev => LoadedScenes.TryAdd(ev.LoadScene.GetHashCode(), ev.LoadScene);

                if (manager.Call())
                    return Result<IScene>.Success(args.LoadScene);
                else
                    return Result<IScene>.Fail();
            }

            return Result<IScene>.Fail();
        }

        public Result<IScene> LoadScene(IScene scene)
        {
            int hash = scene.GetHashCode();
            if (!LoadedScenes.ContainsKey(hash))
            {
                LoadSceneEventArgs args = new LoadSceneEventArgs(this, scene);
                EventManager<LoadSceneEventArgs> manager
                    = new EventManager<LoadSceneEventArgs>(LoadSceneEvent, this, args);
                manager.OnSuccess = ev => LoadedScenes.TryAdd(ev.LoadScene.GetHashCode(), ev.LoadScene);

                if (manager.Call())
                    return Result<IScene>.Success(args.LoadScene);
                else
                    return Result<IScene>.Fail();
            }

            return Result<IScene>.Fail();
        }

        public Result<T> LoadSceneUnsafe<T>(int hash) where T : IScene
        {
            try
            {
                return Result<T>.Success((T) LoadScene(hash).Value);
            }
            catch (Exception e) when (e is InvalidCastException || e is InvalidOperationException)
            {
                return Result<T>.Fail(e.ToString());
            }
        }

        public Result<T> LoadSceneUnsafe<T>(T scene) where T : IScene
        {
            try
            {
                return Result<T>.Success((T) LoadScene(scene).Value);
            }
            catch (Exception e) when (e is InvalidCastException || e is InvalidOperationException)
            {
                return Result<T>.Fail(e.ToString());
            }
        }

        public Result<IScene> UnloadScene(int hash)
        {
            if (LoadedScenes.TryGetValue(hash, out IScene scene))
            {
                UnloadSceneEventArgs args = new UnloadSceneEventArgs(this, scene);
                EventManager<UnloadSceneEventArgs> manager
                    = new EventManager<UnloadSceneEventArgs>(UnloadSceneEvent, this, args);
                manager.OnSuccess = ev => LoadedScenes.TryRemove(args.UnloadScene.GetHashCode(), out scene);

                if (manager.Call())
                    return Result<IScene>.Success(args.UnloadScene);
                else
                    return Result<IScene>.Fail();
            }

            return Result<IScene>.Fail();
        }

        public Result<IScene> UnloadScene(IScene scene)
        {
            return UnloadScene(scene.GetHashCode());
        }

        public Result<T> UnloadSceneUnsafe<T>(int hash) where T : IScene
        {
            try
            {
                return Result<T>.Success((T) UnloadScene(hash).Value);
            }
            catch (Exception e) when (e is InvalidCastException || e is InvalidOperationException)
            {
                return Result<T>.Fail(e.ToString());
            }
        }

        public Result<T> UnloadSceneUnsafe<T>(T scene) where T : IScene
        {
            try
            {
                return Result<T>.Success((T) UnloadScene(scene).Value);
            }
            catch (Exception e) when (e is InvalidCastException || e is InvalidOperationException)
            {
                return Result<T>.Fail(e.ToString());
            }
        }

        public bool UnloadScenes()
        {
            int c = 0;
            foreach (int hash in LoadedScenes.Keys)
            {
                if (UnloadScene(hash).IsSuccess)
                    c++;
            }

            return c > 0;
        }

        public Result<IScene> LoadNextScene(int hash)
        {
            UnloadScenes();
            return LoadScene(hash);
        }

        public Result<IScene> LoadNextScene(IScene scene)
        {
            UnloadScenes();
            return LoadScene(scene);
        }

        public Result<T> LoadNextSceneUnsafe<T>(int hash) where T : IScene
        {
            UnloadScenes();
            try
            {
                return Result<T>.Success((T) LoadScene(hash).Value);
            }
            catch (Exception e) when (e is InvalidCastException || e is InvalidOperationException)
            {
                return Result<T>.Fail(e.ToString());
            }
        }

        public Result<T> LoadNextSceneUnsafe<T>(T scene) where T : IScene
        {
            UnloadScenes();
            try
            {
                return Result<T>.Success((T) LoadScene(scene).Value);
            }
            catch (Exception e) when (e is InvalidCastException || e is InvalidOperationException)
            {
                return Result<T>.Fail(e.ToString());
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            foreach (IScene scene in LoadedScenes.Values)
            {
                foreach (IObject obj in scene.SceneObjects.Values)
                {
                    obj.OnUpdate();
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            foreach (IScene scene in LoadedScenes.Values)
            {
                foreach (IObject obj in scene.SceneObjects.Values)
                {
                    if (obj is IDrawable)
                    {
                        GL.PushAttrib(AttribMask.EnableBit);
                        ((IDrawable) obj).OnDraw();
                        GL.PopAttrib();
                    }
                }
            }

            SwapBuffers();
        }

        protected override void OnLoad(EventArgs e)
        {
            OnInitialze();

            base.OnLoad(e);
        }

        public override void Exit()
        {
            OnDestroy();

            base.Exit();
        }
    }
}