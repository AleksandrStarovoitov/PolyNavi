using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using PolyNavi.Fragments;

namespace PolyNavi.Views
{
    public class MainBuildingView : View
	{
		public enum Marker
		{
			Start,
			End,
			None,
		}

        private readonly Paint routePaint = new Paint() { Color = Color.Blue, StrokeCap = Paint.Cap.Round, StrokeWidth = 7.0f, };
        private readonly Paint startPointPaint = new Paint() { Color = Color.Green };
        private readonly Paint endPointPaint = new Paint() { Color = Color.Red };

        private float[] route;
		//TODO изменить на рисунки
        private Marker marker = Marker.None;
        private Point markerPoint;

		public static bool drawerState = false;
        private static readonly int InvalidPointerId = -1;

        private readonly Drawable plan;
		//readonly ScaleGestureDetector _scaleDetector;
        private readonly GestureDetector doubleTapListener;

        private int activePointerId = -1;
        private float lastTouchX;
        private float lastTouchY;
		public float PosX { get; set; }
		public float PosY { get; set; }

        private readonly float _scaleFactor = 1.0f;
		//float _minScaleFactor = 0.9f;
		//float _maxScaleFactor = 5.0f;

        private DisplayMetrics displ;

        private readonly int baseWidth = 3200;
        private readonly int baseHeight = 1800;

        private float widthScale, heightScale;
        private int imageWidth, imageHeight;

        private Context c;
        private InputMethodManager imm;

		public MainBuildingView(Context context, int id) :
			base(context, null, 0)
		{
			startPointPaint.SetStyle(Paint.Style.Fill); //TODO
			c = context;
			displ = Resources.DisplayMetrics;

			plan = ContextCompat.GetDrawable(Context, id);

			//imageWidth = _plan.IntrinsicWidth;
			//imageHeight = _plan.IntrinsicHeight;

            imageWidth = (int)(displ.HeightPixels * 1.777778 * 0.85);
		    imageHeight = (int)(displ.HeightPixels * 0.85);

            widthScale = (float)imageWidth / baseWidth;
			heightScale = (float)imageHeight / baseHeight;

			routePaint.StrokeWidth = routePaint.StrokeWidth * widthScale;

			//if (displ.HeightPixels - imageHeight > 0 && displ.HeightPixels - imageHeight < 50)
			//{
			//	_scaleFactor *= 0.9f;
			//}
            
            plan.SetBounds(0, 0, imageWidth, imageHeight);
			//_scaleDetector = new ScaleGestureDetector(context, new MyScaleListener(this));
			doubleTapListener = new GestureDetector(context, new MyDoubleTapListener(this, displ));

			imm = (InputMethodManager)c.GetSystemService(Context.InputMethodService);
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
				imm.HideSoftInputFromWindow(WindowToken, 0);
			}
			//_scaleDetector.OnTouchEvent(e);
			doubleTapListener.OnTouchEvent(e);

			MotionEventActions action = e.Action & MotionEventActions.Mask;
			int pointerIndex;

            switch (action)
            {
                case MotionEventActions.Down:
                    lastTouchX = e.GetX();
                    lastTouchY = e.GetY();
                    activePointerId = e.GetPointerId(0);
                    break;

                case MotionEventActions.Move:
                    pointerIndex = e.FindPointerIndex(activePointerId);
                    float x = e.GetX(pointerIndex);
                    float y = e.GetY(pointerIndex);
                    //if (!_scaleDetector.IsInProgress)
                    //{
                    //Only move the ScaleGestureDetector isn't already processing a gesture.
                    float deltaX = x - lastTouchX;
                    float deltaY = y - lastTouchY;
                    PosX += deltaX;
                    PosY += deltaY;
                    
					float planScaleWidth = imageWidth * _scaleFactor;
					float planScaleHeight = imageHeight * _scaleFactor;

					float right = PosX + planScaleWidth;
					float left = PosX;
					float top = PosY;
					float bottom = PosY + planScaleHeight;

                    Log.Debug("PLAN", "Right: " + right);
                    Log.Debug("PLAN", "Left: " + left);
                    Log.Debug("PLAN", "Top: " + top);
                    Log.Debug("PLAN", "Bottom: " + bottom + " // IntristicHeight: " + plan.IntrinsicHeight + " // ImageHeight: " + imageHeight);
                    
                    if (right < displ.WidthPixels)
					{
						PosX -= deltaX;
					}
					if (left > 0)
					{
						PosX -= deltaX;
					}
					if (top > 0)
					{
						PosY -= deltaY;
					}
					if (bottom < imageHeight)
					{
						PosY -= deltaY;
					}

					Invalidate();
					//}

					lastTouchX = x;
					lastTouchY = y;
					break;

				case MotionEventActions.Up:
				case MotionEventActions.Cancel:
					activePointerId = InvalidPointerId;
					break;

				case MotionEventActions.PointerUp:
					pointerIndex = (int)(e.Action & MotionEventActions.PointerIndexMask) >> (int)MotionEventActions.PointerIndexShift;
					int pointerId = e.GetPointerId(pointerIndex);
					if (pointerId == activePointerId)
					{
						int newPointerIndex = pointerIndex == 0 ? 1 : 0;
						lastTouchX = e.GetX(newPointerIndex);
						lastTouchY = e.GetY(newPointerIndex);
						activePointerId = e.GetPointerId(newPointerIndex);
					}
					break;
			}
			return true;
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
			canvas.Save();
			canvas.Translate(PosX, PosY);
			canvas.Scale(_scaleFactor, _scaleFactor);
			plan.Draw(canvas);
			if (route != null)
			{
				canvas.DrawLines(route, routePaint);
			}
			if (marker == Marker.Start)
			{
				canvas.DrawCircle(markerPoint.X, markerPoint.Y, 10.0f * widthScale, startPointPaint);
			}
			if (marker == Marker.End)
			{
				canvas.DrawRect(markerPoint.X - 10.0f * widthScale, markerPoint.Y - 10.0f * heightScale, markerPoint.X + 10.0f * widthScale, markerPoint.Y + 10.0f * heightScale, endPointPaint);
			}
			canvas.Restore();
		}

		public void SetMarker(Point point, Marker marker)
		{
			this.marker = marker;

			point.X = (int)(point.X * widthScale);
			point.Y = (int)(point.Y * heightScale);

			markerPoint = point;
		}

		public void SetRoute(IList<Point> points)
		{
            var r = new List<float>();
            if (route != null)
            {
                r = route.ToList();
            }
			if (points == null)
			{
				route = new float[0];
			}
			else
			{
				int segmentsCount = points.Count - 1;
				route = new float[segmentsCount * 4];
				route[0] = points[0].X * widthScale;
				route[1] = points[0].Y * heightScale;
				int i;
				int j;
				for (i = 1, j = 2; i < points.Count - 1; ++i, j += 4)
				{
					route[j] = points[i].X * widthScale;
					route[j + 1] = points[i].Y * heightScale;
					route[j + 2] = points[i].X * widthScale;
					route[j + 3] = points[i].Y * heightScale;
				}
				route[j] = points[i].X * widthScale;
				route[j + 1] = points[i].Y * heightScale;

				if (r != null)
                {
                    r.AddRange(route.ToList());
                    route = new float[r.Count];
                    route = r.ToArray();
                }
			}
		}

        private class MyDoubleTapListener : GestureDetector.SimpleOnGestureListener
        {
            private MainBuildingView view;
            private bool zoomedIn = false;
            private DisplayMetrics displ;

            public MyDoubleTapListener(MainBuildingView view, DisplayMetrics displ)
            {
                this.view = view;
                this.displ = displ;
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                return base.OnDoubleTap(e);
            }
        }



        //private class MyScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
        //{
        //	readonly MainBuildingView _view;
        //	float centerX;
        //	float centerY;
        //	float deltaX;
        //	float deltaY;

        //	float planScaleWidth;
        //	float planScaleHeight;
        //	float right;
        //	float left;
        //	float top;
        //	float bottom;

        //	public MyScaleListener(MainBuildingView view)
        //	{
        //		_view = view;
        //	}

        //	public override bool OnScale(ScaleGestureDetector detector)
        //	{

        //		float scale = detector.ScaleFactor;

        //		_view._scaleFactor = System.Math.Max(_view._minScaleFactor, System.Math.Min(_view._scaleFactor * scale, _view._maxScaleFactor));

        //		if (_view._scaleFactor > _view._minScaleFactor && _view._scaleFactor < _view._maxScaleFactor)
        //		{
        //			centerX = detector.FocusX;
        //			centerY = detector.FocusY;
        //			deltaX = centerX - _view._posX;
        //			deltaY = centerY - _view._posY;
        //			deltaX = deltaX * scale - deltaX;
        //			deltaY = deltaY * scale - deltaY;

        //			planScaleWidth = _view._plan.IntrinsicWidth * _view._scaleFactor;
        //			planScaleHeight = _view._plan.IntrinsicHeight * _view._scaleFactor;

        //			right = _view._posX + planScaleWidth;
        //			left = _view._posX;
        //			top = _view._posY;
        //			bottom = _view._posY + planScaleHeight;

        //			//Log.Debug("plan", "right: " + right.ToString());
        //			//Log.Debug("plan", "left: " + left.ToString());
        //			//Log.Debug("plan", "top: " + top.ToString());
        //			//Log.Debug("plan", "bottom: " + bottom.ToString());

        //			if (right < _view.displ.WidthPixels)
        //			{
        //				_view._posX -= deltaX;
        //			}
        //			if (left > 0)
        //			{
        //				_view._posX += deltaX;
        //			}
        //			if (top > 0)
        //			{
        //				_view._posY += deltaY;
        //			}
        //			if (bottom < _view._plan.IntrinsicHeight)
        //			{
        //				_view._posY -= deltaY;
        //			}

        //			_view._posX -= deltaX;
        //			_view._posY -= deltaY;
        //		}

        //		_view.Invalidate();
        //		return true;
        //	}
        //}
    }
}