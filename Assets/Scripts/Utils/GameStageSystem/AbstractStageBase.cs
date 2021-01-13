using System;
using System.Collections.Generic;

namespace Defong.GameStageSystem
{
    /// <summary>
    /// Abstract class to be inherited for all game stages
    /// </summary>
    public abstract class AbstractStageBase : IStage
    {
        public abstract object StageId { get; }

        public virtual event EventHandler<StageCallbackEventArgs> OnStageFinished;

        public Dictionary<object, IStage> SubStages { get; } = new Dictionary<object, IStage>();

        /// <summary>
        /// Automatically called when entering the stage
        /// </summary>
        /// <param name="data"></param>
        public virtual void Initialize(object data)
        {
            foreach (var value in SubStages.Values)
            {
                value.Initialize(data);
            }
        }

        /// <summary>
        /// Method to be called when exiting the stage.
        /// </summary>
        public virtual void DeInitialize()
        {
            foreach (var value in SubStages.Values)
            {
                value.DeInitialize();
            }
        }

        /// <summary>
        /// Raises the OnStageLoaded event
        /// </summary>
        /// <param name="sender">Object that raises the event</param>
        /// <param name="args">Event args</param>
        protected virtual void StageFinished(object sender, StageCallbackEventArgs args)
        {
            OnStageFinished?.Invoke(sender, args);
        }

        /// <summary>
        /// Loads the stage and raises callback when done
        /// </summary>
        public abstract void Show();

        public abstract void Hide();
    }

    public class StageCallbackEventArgs : EventArgs
    {
        public object ObjArgs { get; set; }
    }
}
