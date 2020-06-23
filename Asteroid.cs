using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace LinhPham_AsteroidLab
{
    //As I encountered with the generating a same random value problem, I decied to make an Ulti helpre class for it
     public static class Ulti
    {
       public static readonly Random _random= new Random();
    }
    abstract public class ShapeBase
    {
        //a constant variable for radius
        public const double _cfRadius = 20;
        //rotation of the shape
        public float _fRotation;
        //rotation variance of shape
        public float _fRotationIncrement;
        //speed change for loation
        public float _fXSpeed;
        public float _fYSpeed;
        //the current of of shape
        private PointF currentPoint;
      // static  Random Ulti.rnd = new Random();
        //used to set color for the shape
        public Color baseColor
        {
            get; set;
        }
        //use for determining collapse
        public bool IsMarkedForDeath
        {
            get; set;
        }
        //property for location of the shape
        public PointF Location
        {
            get
            {
                return currentPoint;
            }
            set
            {
                currentPoint = value;
            }
        }

        //a constructor accepts a point
        public ShapeBase(PointF p)
        {
            //set the back color to be black
            baseColor = Color.Black;
            currentPoint = p;
            //  Ulti.rnd = new Random();
            //the current rotation as 0
            _fRotation = 0;
            //generate random number from -3 to 3
            //random*(max-min)+min
            _fRotationIncrement = 0;
            //X,Y speed have random values from -2.5 to 2.5
            _fXSpeed = (float)(Ulti._random.NextDouble() * 5.0 - 2.5);
            _fYSpeed = (float)(Ulti._random.NextDouble() * 5.0 - 2.5);
            IsMarkedForDeath = false;
        }
      

        //an abstract method to be used to get graphic path of each shape
        abstract public GraphicsPath GetPath();
        //an abstract method to be used to get region of each shape, used for determine intersaction between shape
        abstract public Region GetRegion();

        /// /////////////////////////////////////////////////////////////////////
        //Method: public void Tick(Size s)
        //Parameter: Size s- the size of canvas window
        //Return: none
        //Purpose: used to move the shape according to the current speed values
        /////////////////////////////////////////////////////////////////////////
        public virtual void Tick(Size s)
        {
            //check for out of bound. if it is, flip over
            if (currentPoint.X + _fXSpeed < 0 || currentPoint.X + _fXSpeed > s.Width)
                _fXSpeed *= -1;
            else if (currentPoint.Y + _fYSpeed < 0 || currentPoint.Y + _fYSpeed > s.Height)
                _fYSpeed *= -1;

            //move location of the shape
            currentPoint.X += _fXSpeed;
            currentPoint.Y += _fYSpeed;
            _fRotation += _fRotationIncrement;
        }
        /// /////////////////////////////////////////////////////////////////////
        //Method:  public void Render(BufferedGraphics bgp)
        //Parameter: BufferedGraphics bgp
        //Return: none
        //Purpose:simply fill the GetPath return value with a provided colour
        /////////////////////////////////////////////////////////////////////////
        public void Render(BufferedGraphics bgp)
        {
            bgp.Graphics.DrawPath(new Pen(baseColor), GetPath());

        }
        ////////////////////////////////////////////////////////////////////////
        //Method:     public static double Dist(ShapeBase s1, ShapeBase s2)
        //Parameter: hapeBase s1, ShapeBase s2- accept two shapes to check their distance
        //Return: double- distance between two shape
        //Purpose:Find the ddistance between two shapes
        /////////////////////////////////////////////////////////////////////////
        public static double Dist(ShapeBase s1, ShapeBase s2)
        {
            double distSq = Math.Sqrt(Math.Pow((s1.Location.X - s2.Location.X), 2) + Math.Pow((s1.Location.Y - s2.Location.Y), 2));
            return distSq;
        }


        ////////////////////////////////////////////////////////////////////////
        //Method:     public bool Collapse(ShapeBase b, Graphics gr)
        //Parameter: ShapeBase- a shape base to check intersection with the current shapebase
        //           Graphics
        //Return: bool collapse- determinne if collapse if happen
        //Purpose:Find the ddistance between two shapes
        /////////////////////////////////////////////////////////////////////////
        public bool Collapse(ShapeBase b, Graphics gr)
        {
            bool collapse = false;
            Region regA = b.GetRegion().Clone();
            regA.Intersect(this.GetRegion().Clone());
            if (!regA.IsEmpty(gr))
            {
                collapse = true;
            }
            return collapse;
        }

        /// /////////////////////////////////////////////////////////////////////
        //Method: public static GraphicsPath MakePolyPath(double radius, int vertexCount, double variance)
        //Parameter: double radius- radius of the shape
        //           int vertex count- the number of vertex for the shape
        //          double variance- the change...
        //Return: GraphicsPath- the graphic path connected by all the points
        //Purpose: Generate a GraphicsPath from your collection of generated points and return i
        /////////////////////////////////////////////////////////////////////////
        public static GraphicsPath MakePolyPath(double radius, int vertexCount, double variance)
        {
            Random rnd = new Random();
            GraphicsPath gp = new GraphicsPath();
            PointF[] myList = new PointF[vertexCount];
            //find all the point aof the vertex
            for (int i = 0; i < vertexCount; i++)
            {
                myList[i] = (new PointF((float)((radius - rnd.NextDouble() * variance) * Math.Cos(2.0 * Math.PI * ((double)(i) / (double)(vertexCount)))),
                   (float)((radius - rnd.NextDouble() * variance) * Math.Sin(2.0 * Math.PI * ((double)(i) / (double)(vertexCount))))
                    ));
            }

            gp.AddLine(myList[myList.Count() - 2], myList.Last());
            gp.StartFigure();
            gp.AddLines(myList);
            gp.CloseFigure();
            return gp;
        }
    }


    public class Ship : ShapeBase
    {
      public  int _ShipRadius;
        //describe the model of the object to be drawn
        private static readonly GraphicsPath _model = new GraphicsPath();
        // static constructor that use MakePolyPath to set the static GraphicsPath witha model of an equilateral Ship
        PointF centerShip;
        static Ship()
        {
            _model = (GraphicsPath)(MakePolyPath(_cfRadius, 3, 0)).Clone();
        }

        public Ship(PointF point) : base(point)
        {
            _ShipRadius = 30;
            centerShip = point;
        }
        /// /////////////////////////////////////////////////////////////////////
        //Method:   public override GraphicsPath GetPath()
        //Parameter: none
        //Return: GraphicsPath- the fully tranformed graphic path of Ship
        //Purpose:clone the model and apply transforms for rotation and translation, returning it 
        /////////////////////////////////////////////////////////////////////////
        public override GraphicsPath GetPath()
        {
          
            // make another pen
            Pen pBlue = new Pen(Color.Blue, 2);
            // build a path, add elements
            GraphicsPath gpb = new GraphicsPath();
            gpb.AddLine(new PointF(centerShip.X - _ShipRadius, centerShip.Y), new PointF((float)(centerShip.X + (_ShipRadius * .3)), centerShip.Y + _ShipRadius/2));
            gpb.AddLine(new PointF(centerShip.X - _ShipRadius, centerShip.Y), new PointF((float)(centerShip.X + (_ShipRadius * .3)), centerShip.Y - _ShipRadius/2));
            gpb.AddLine(new PointF(centerShip.X, centerShip.Y), new PointF((float)(centerShip.X + (30 * .3)), centerShip.Y + 15));

            Matrix mat = new Matrix(); // Object to load transformations
                                       // mat.Translate(50, 10); // over 50, down 10
                                       // apply the transform to the path

            mat.RotateAt(_fRotation, centerShip);
            gpb.Transform(mat);
            return gpb;
        }
        /// /////////////////////////////////////////////////////////////////////
        //Method:    public override Region GetRegion()
        //Parameter: none
        //Return: Region- the region of the current shape
        //Purpose: For checking intersetion purpose
        /////////////////////////////////////////////////////////////////////////
        public override Region GetRegion()
        {
            return new Region(GetPath());
        }

    }

   public class Bullet: Ship
    {

        PointF _bulletLoc;
        float angle;
        /// /////////////////////////////////////////////////////////////////////
        //Constructor:  public Bullet(PointF p, float rotation): base(p)
        //Parameter: point p- the current point of bullet
        //          rotation- to determine how the bullet move when fired out from the tip of the ship
        /////////////////////////////////////////////////////////////////////////
        public Bullet(PointF p, float rotation): base(p)
        {
            _bulletLoc = p;
            angle = rotation;
        }
        public override GraphicsPath GetPath()
        {
            // make another pen
            Pen pBlue = new Pen(Color.Blue, 2);
            // build a path, add elements
            GraphicsPath gpb = new GraphicsPath();
            //   gpb.AddEllipse(20, 10, 10, 10); // startx, starty, width, height
            gpb.StartFigure();
            gpb.AddEllipse(new RectangleF(new PointF(0,0), new Size(2, 2)));
            gpb.CloseFigure();
            Matrix mat = new Matrix(); // Object to load transformations
                                       // mat.Translate(50, 10); // over 50, down 10
                                       // apply the transform to the path
                                       //    mat.Translate(xIncre,yIncre);
                                       //mat.Translate(centerShip, centerShip.Y);
            mat.Translate(_bulletLoc.X , _bulletLoc.Y);
            gpb.Transform(mat);

            return gpb;
        }
        /// /////////////////////////////////////////////////////////////////////
        //Method:    public override Region GetRegion()
        //Parameter: none
        //Return: Region- the region of the current shape
        //Purpose: For checking intersetion purpose
        /////////////////////////////////////////////////////////////////////////
        public override Region GetRegion()
        {
            return new Region(GetPath());
        }

        /// /////////////////////////////////////////////////////////////////////
        //Method:   public override void Tick(Size s)
        //Parameter: Size s- not using anything here. but need in parameter as its derived from main class
        //Return: none
        //Purpose: To update the location of the bullet
        /////////////////////////////////////////////////////////////////////////
        public override void Tick(Size s)
         {
            float x = (float)-Math.Cos(angle * Math.PI / 180) * 10 ;
            float y = (float)-Math.Sin(angle * Math.PI / 180) * 10;

            _bulletLoc = new PointF(_bulletLoc.X + x, _bulletLoc.Y + y);
        }

    }
    public class Asteroid : ShapeBase
    {

        //describe the model of the object to be drawn
        GraphicsPath _model = new GraphicsPath();
        int _size;

        /// ////////////////////////////////////////////////////////////////////////////////////////////
        //Constructor:    public Asteroid(PointF point, int s) : base(point)
        //Parameter: point p- the current point of bullet
        //           s- the wanted size for theasteroid
        ////////////////////////////////////////////////////////////////////////////////////////////////
        public Asteroid(PointF point, int s) : base(point)
        {
            _size = s;
        }
        public int RockSize
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }
        /// /////////////////////////////////////////////////////////////////////
        //Method:   public override GraphicsPath GetPath()
        //Parameter: none
        //Return: GraphicsPath- the fully tranformed graphic path of Ship
        //Purpose:clone the model and apply transforms for rotation and translation, returning it 
        /////////////////////////////////////////////////////////////////////////
        public override GraphicsPath GetPath()
        {
            //rocck with 6 to 12 vertexs and variance of 30%
            GraphicsPath gp = (GraphicsPath)(MakePolyPath(_size,  Ulti._random.Next(6, 12), 0.3)).Clone();
            Matrix tranforms = new Matrix();
            tranforms.Translate(base.Location.X, base.Location.Y);
            // tranforms.Translate(20, 20);
            tranforms.Rotate((float)(base._fRotation));
            gp.Transform(tranforms);
            return gp;
        }
        /// /////////////////////////////////////////////////////////////////////
        //Method:    public override Region GetRegion()
        //Parameter: none
        //Return: Region- the region of the current shape
        //Purpose: For checking intersetion purpose
        /////////////////////////////////////////////////////////////////////////
        public override Region GetRegion()
        {
            return new Region(GetPath());
        }

    }

   
}
