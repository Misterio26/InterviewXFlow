using Lime;
using Lime.KineticMotionStrategy;
using Robot.Core.Common.Camera;
using Robot.Core.Common.Utils;
using Robot.Layer1.Common.ActivitiesSystem;
using Robot.Layer2.Common;

namespace Exmaple
{
	public class ControllerUserInputToCamera : IController
	{
		private const float WheelZoomFactor = 1.1f;

		private readonly List<Widget> inputOwners;
		private readonly Camera camera;
		private readonly List<DragGesture> dragGestures;
		private readonly List<PinchGesture> pinchGestures;
		
		public bool subscribed;
		
		public bool Enabled;
		
		public ControllerUserInputToCamera(Camera camera, List<Widget> inputOwners)
		{
			this.inputOwners = inputOwners;
			this.camera = camera;

			dragGestures = new List<DragGesture>();
			pinchGestures = new List<PinchGesture>();
		}

		void IController.OnStart()
		{
			foreach (var inputOwner in inputOwners) {
				var dragGesture = new KineticDragGesture(new DeceleratingKineticMotionStrategy(0.97f, 1.002f));
				var pinchGesture = new PinchGesture(exclusive: true);

				inputOwner.Gestures.Add(dragGesture);
				dragGestures.Add(dragGesture);
				inputOwner.Gestures.Add(pinchGesture);
				pinchGestures.Add(pinchGesture);
			}
		}

		void IController.OnStop()
		{
			for (int i = 0; i < inputOwners.Count; i++) {
				var inputOwner = inputOwners[i];
				inputOwner.Gestures.Remove(dragGestures[i]);
				inputOwner.Gestures.Remove(pinchGestures[i]);
			}
			
			if (subscribed)
			{
				subscribed = false;
				UnsubscribeFromGestures();
			}
		}

		void IController.OnUpdate()
		{
			if (Enabled)
			{
				if (!subscribed)
				{
					subscribed = true;
					SubscribeOnGestures();
				}
			}
			else
			{
				if (subscribed)
				{
					subscribed = false;
					UnsubscribeFromGestures();
				}
			}
		}

		private void SubscribeOnGestures()
		{
			for (int i = 0; i < inputOwners.Count; i++) {
				var dragGesture = dragGestures[i];
				var pinchGesture = pinchGestures[i];
				dragGesture.Changed += OnDragged;
				pinchGesture.Changed += OnPinched;
			}
		}

		private void UnsubscribeFromGestures()
		{
			for (int i = 0; i < inputOwners.Count; i++) {
				var dragGesture = dragGestures[i];
				var pinchGesture = pinchGestures[i];
				dragGesture.Changed -= OnDragged;
				pinchGesture.Changed -= OnPinched;
			}
		}

		private void OnPinched()
		{
			for (int i = 0; i < inputOwners.Count; i++) {
				var pinchGesture = pinchGestures[i];
				if (pinchGesture.IsActive) {
					camera.Position -= pinchGesture.LastDragDistance / camera.Zoom;
					float zoom = ClampZoom(camera.Zoom * pinchGesture.LastPinchScale);
					ZoomOrigin(pinchGesture.MousePosition, zoom);
				}
			}
		}

		private void OnDragged()
		{
			for (int i = 0; i < inputOwners.Count; i++) {
				var dragGesture = dragGestures[i];
				if (dragGesture.IsActive) {
					camera.Position -= dragGesture.LastDragDistance / camera.Zoom;
				}
			}
		}

		private void ZoomOrigin(in Vector2 origin, float zoom)
		{
			float zoomDelta = zoom / camera.Zoom;
			var originInViewport = origin * camera.CalcGlobalToLocalTransform();
			var offset = originInViewport * (1.0f - 1.0f / zoomDelta);
			camera.Position += offset;
			camera.Zoom = zoom;
		}

		private float ClampZoom(float desiredZoom)
		{
			return desiredZoom.Clamp(settings.MinZoom, settings.MaxZoom);
		}
	}
}
