using System;
using System.Drawing;
using EngineGL.GraphicEngine;
using OpenTK;

namespace EngineGL.Window
{
    public interface IWindow
    {
        IGraphicAdapter GraphicAdapter { get; }

        Icon Icon { get; set; }
        string Title { get; set; }

        void AddLoadEvent(EventHandler<EventArgs> handler);
        void RemoveLoadEvent(EventHandler<EventArgs> handler);

        void AddUnloadEvent(EventHandler<EventArgs> handler);
        void RemoveUnloadEvent(EventHandler<EventArgs> handler);

        void AddRenderFrameEvent(EventHandler<FrameEventArgs> handler);
        void RemoveRenderFrameEvent(EventHandler<FrameEventArgs> handler);

        void AddUpdateFrameEvent(EventHandler<FrameEventArgs> handler);
        void RemoveUpdateFrameEvent(EventHandler<FrameEventArgs> handler);

        void Run();
    }
}