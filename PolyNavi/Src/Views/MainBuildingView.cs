using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Support.V4.Content;
using Android.Graphics;
using Android.Views.InputMethods;

namespace PolyNavi
{
	public class MainBuildingView : View
	{
		public static bool drawerState = false;
		private static readonly int InvalidPointerId = -1;

		private readonly Drawable _plan;
		private readonly ScaleGestureDetector _scaleDetector;

		private int _activePointerId = -1;
		private float _lastTouchX;
		private float _lastTouchY;
		private float _posX;
		private float _posY;
		private float _scaleFactor = 0.4f;
		private float _minScaleFactor = 0.3f;
		private float _maxScaleFactor = 5.0f;

		Android.Util.DisplayMetrics displ;
		int widthInDp;
		int heightInDp;

		Context c;
		InputMethodManager imm;



		public MainBuildingView(Context context) :
			base(context, null, 0)
		{
			c = context;
			displ = Resources.DisplayMetrics;
			widthInDp = ConvertPixelsToDp(displ.WidthPixels);
			heightInDp = ConvertPixelsToDp(displ.HeightPixels);


			_plan = ContextCompat.GetDrawable(context, Resource.Drawable.first_plan1);
			_plan.SetBounds(0, 0, _plan.IntrinsicWidth, _plan.IntrinsicHeight);
			_scaleDetector = new ScaleGestureDetector(context, new MyScaleListener(this));
			imm = (InputMethodManager)c.ApplicationContext.GetSystemService(Context.InputMethodService);
		}

		private int ConvertPixelsToDp(float pixelValue)
		{
			var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
			return dp;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{

			if (!MainBuildingFragment.CheckFocus())
			{
				//InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
				imm.HideSoftInputFromWindow(WindowToken, 0);
			}
			_scaleDetector.OnTouchEvent(e);

			MotionEventActions action = e.Action & MotionEventActions.Mask;
			int pointerIndex;

			switch (action)
			{
				case MotionEventActions.Down:
					_lastTouchX = e.GetX();
					_lastTouchY = e.GetY();
					_activePointerId = e.GetPointerId(0);
					break;

				case MotionEventActions.Move:
					pointerIndex = e.FindPointerIndex(_activePointerId);
					float x = e.GetX(pointerIndex);
					float y = e.GetY(pointerIndex);
					if (!_scaleDetector.IsInProgress)
					{
						//Only move the ScaleGestureDetector isn't already processing a gesture.
						float deltaX = x - _lastTouchX;
						float deltaY = y - _lastTouchY;
						_posX += deltaX;
						_posY += deltaY;

						Invalidate();
					}

					_lastTouchX = x;
					_lastTouchY = y;
					break;

				case MotionEventActions.Up:
				case MotionEventActions.Cancel:
					// This events occur when something cancels the gesture (for example the
					//activity going in the background) or when the pointer has been lifted up.
					//We no longer need to keep track of the active pointer.
					_activePointerId = InvalidPointerId;
					break;

				case MotionEventActions.PointerUp:
					//We only want to update the last touch position if the the appropriate pointer
					//has been lifted off the screen.
					pointerIndex = (int)(e.Action & MotionEventActions.PointerIndexMask) >> (int)MotionEventActions.PointerIndexShift;
					int pointerId = e.GetPointerId(pointerIndex);
					if (pointerId == _activePointerId)
					{
						//This was our active pointer going up. Choose a new
						//action pointer and adjust accordingly
						int newPointerIndex = pointerIndex == 0 ? 1 : 0;
						_lastTouchX = e.GetX(newPointerIndex);
						_lastTouchY = e.GetY(newPointerIndex);
						_activePointerId = e.GetPointerId(newPointerIndex);
					}
					break;
			}
			return true;
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);


			canvas.Save();
			canvas.Translate(_posX, _posY);
			canvas.Scale(_scaleFactor, _scaleFactor);
			_plan.Draw(canvas);
			canvas.Restore();

		}


		private class MyScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
		{
			private readonly MainBuildingView _view;

			public MyScaleListener(MainBuildingView view)
			{
				_view = view;
			}

			public override bool OnScale(ScaleGestureDetector detector)
			{

				float scale = detector.ScaleFactor;

				_view._scaleFactor = System.Math.Max(_view._minScaleFactor, System.Math.Min(_view._scaleFactor * scale, _view._maxScaleFactor));

				if (_view._scaleFactor > _view._minScaleFactor && _view._scaleFactor < _view._maxScaleFactor)
				{
					float centerX = detector.FocusX;
					float centerY = detector.FocusY;
					float deltaX = centerX - _view._posX;
					float deltaY = centerY - _view._posY;
					deltaX = deltaX * scale - deltaX;
					deltaY = deltaY * scale - deltaY;
					_view._posX -= deltaX;
					_view._posY -= deltaY;
				}

				_view.Invalidate();
				return true;
			}
		}
	}
}