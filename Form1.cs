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
    public partial class Game : Form
    {
        //3 different size of asteroid
        int largeSize = 30;
        int mediumSize = 20;
        int smallSize = 10;
        //3 diffirent point for each point
        int largeMark = 0;
        int mediumMark = 0;
        int smallMark = 0;
        //total points
        int totalMark = 0;
        //a temporary point to check for every 1000 points
        int tempMark = 0;
        //number of ship at the beginning
        int currentLife = 3;
        //number of bullet for each ship at the beginning 
        int bulletNumer = 8;
        //boolean variable to determine when player run out of ship
        bool loser = false;
        //boolean to stop/resume the game
        bool Stop = false;
        //The ship of the game
        Ship userShip;
        //list of asteroid that contains all size of asteroid
        List<Asteroid> _listAsteroid = new List<Asteroid>();
        //list of bullet
        List<Bullet> _bulletList = new List<Bullet>();
        //boolean variable to detertime the game starts
        bool gameStart = false;
        //instruction when game start
        string instruction = "Welcome to my Asteroid Game\nUp key to rotate ship in clockwise\nDown key to rotate the ship in anti-clockwise\nPress U to pause or resume the game\nPress Enter to start a new game";
        public Game()
        {
            InitializeComponent();
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //set value to be 1000
            this.Width = 1000;
            this.Height = 1000;
            GameLoad();
        }
        /////////////////////////////////////////////////////////
        /// Method: string UpdateScore()
        /// Parameter: none
        /// Purpose: Update the current game's points
        /// Return: A string to display current point to user
        /////////////////////////////////////////////////////////
        string UpdateScore()
        {
            totalMark = largeMark + mediumMark + smallMark;
            string score = $"Current points: {totalMark}";
            return score;
        }

        /////////////////////////////////////////////////////////
        /// Method:     string UpdateLife()
        /// Parameter: none
        /// Purpose: Update the current number of ships
        /// Return: A string to display current ship number to user
        /////////////////////////////////////////////////////////
        string UpdateLife()
        {
            string score = $"Current Ship number: {currentLife}";
            return score;
        }
        /////////////////////////////////////////////////////////
        /// Method:      string UpdateBullet()
        /// Parameter: none
        /// Purpose: Update the current bullet of the current ship
        /// Return: A string to display current ship number to user
        /////////////////////////////////////////////////////////
        string UpdateBullet()
        {
            string score = $"Be careful. You only have {bulletNumer} bullets left";
            return score;
        }

        /////////////////////////////////////////////////////////
        /// Method:  void GameLoad()
        /// Parameter: none
        /// Purpose: Reset all variale and prepare for a new game
        /// Return: nothing
        /////////////////////////////////////////////////////////
        void GameLoad()
        {
            //reset all value when game is played again
            gameStart = false;
            loser = false;
            largeSize = 30;
            mediumSize = 20;
            smallSize = 10;
            largeMark = 0;
            mediumMark = 0;
            smallMark = 0;
            currentLife = 3;
            totalMark = 0;
            tempMark = 0;
            bulletNumer = 8;
           _listAsteroid = new List<Asteroid>();
           _bulletList = new List<Bullet>();
            //a timer for the game to run
            Timer GameTimer = new Timer();
            GameTimer.Tick += GameTimer_Tick;
            GameTimer.Interval = 25;
            GameTimer.Start();
            //game is configured to let the asteroid start at reasonable location for user
            //add 4 asteroid in the left side
            for (int a = 0; a < 4; a++)
            {
                _listAsteroid.Add(new Asteroid(new PointF(Ulti._random.Next(0, (int)(this.Width*(1.0/4))), Ulti._random.Next(0, (int)(this.Height))), largeSize));
            }
            //add 4 asteroid in the up side
            for (int a = 0; a < 4; a++)
            {
                _listAsteroid.Add(new Asteroid(new PointF(Ulti._random.Next((int)(this.Width * (1.0 / 4)), (int)(this.Width * (3.0 / 4))), Ulti._random.Next(0, (int)(this.Height * (1.0 / 4)))), largeSize));
            }
            //add 4 asteroid in the right side
            for (int a = 0; a < 4; a++)
            {
                _listAsteroid.Add(new Asteroid(new PointF(Ulti._random.Next((int)(this.Width * (3.0 / 4)), (int)this.Width), Ulti._random.Next(0, this.Height)), largeSize));
            }
            //add 4 asteroid in the under
            for (int a = 0; a < 4; a++)
            {
                _listAsteroid.Add(new Asteroid(new PointF(Ulti._random.Next((int)(this.Width * (1.0 / 4)), (int)(this.Width * (3.0 / 4))), Ulti._random.Next((int)(this.Height * (3.0 / 4)), (int)this.Height)), largeSize));
            }
            //a userShip is added in the middle of the screen
            userShip = new Ship(new PointF(this.Width / 2, this.Height / 2));
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            //check if user run out of ship
            //user lost
            if (currentLife == 0)
            {
                loser = true;
            }

            Graphics gr = CreateGraphics();
            using (BufferedGraphicsContext bgc = new BufferedGraphicsContext())
            {
                using (BufferedGraphics bg = bgc.Allocate(gr, ClientRectangle))
                {
                    if (gameStart)
                    {
                        //if user still has ship to play
                        if (!loser)
                        {
                            //if user not pause the game
                            if (!Stop)
                            {
                                //clear the buffer and smooth object
                                bg.Graphics.Clear(Color.Black);
                                bg.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                                //ship is displayed in white
                                userShip.baseColor = Color.White;
                                //render the ship
                                userShip.Render(bg);
                                //determine asteroid size to assign color to it
                                foreach (Asteroid a in _listAsteroid)
                                {
                                    //Large ->Red   
                                    if (a.RockSize == largeSize)
                                        a.baseColor = Color.Red;
                                    //Medium ->Yellow
                                    if (a.RockSize == mediumSize)
                                        a.baseColor = Color.Yellow;
                                    //Small ->Blue
                                    if (a.RockSize == smallSize)
                                        a.baseColor = Color.Blue;
                                }
                                //all bullet is displayed in yellow
                                foreach (Bullet b in _bulletList)
                                    b.baseColor = Color.Yellow;
                                //as we modify the list when checking the intersection, getiing copy of each list to iterate through is a must
                                List<Asteroid> tempAsteroidList = _listAsteroid.ToList();
                                List<Bullet> tempBulletList = _bulletList.ToList();

                                //check for intersection betwwen asteroid and bullet
                                //iterate through asteroid list
                                foreach (Asteroid a in tempAsteroidList)
                                {
                                    //iterate through  bullet list
                                    foreach (Bullet b in tempBulletList)
                                    {
                                        //check collision of each bullet with the current asteroid
                                        if (a.Collapse(b, gr))
                                        {
                                            //if collapse, they are both marked to be death
                                            a.IsMarkedForDeath = true;
                                            b.IsMarkedForDeath = true;
                                            //if asteroid is death, it will be broken down or deleted depends on the size
                                            //add 2 medium rocks if its preivous is a large rock. Determine for the point for each rocck size broken down here as well
                                            if (a.RockSize == largeSize)
                                            {
                                                largeMark += 100;
                                                for (int j = 0; j < 2; j++)
                                                {
                                                    _listAsteroid.Add(new Asteroid(a.Location, mediumSize));

                                                }
                                            }
                                            //add 2 small rocks if its preivous is a medium rock
                                            if (a.RockSize == mediumSize)
                                            {
                                                mediumMark += 200;

                                                for (int j = 0; j < 2; j++)
                                                {
                                                    _listAsteroid.Add(new Asteroid(a.Location, smallSize));

                                                }
                                            }
                                            //mark it death to delete it after if its a small rock
                                            if (a.RockSize == smallSize)
                                                smallMark += 300;
                                            break;

                                        }
                                    }
                                }
                                //update score to the user
                                bg.Graphics.DrawString(UpdateScore(), new Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Red), 10, 10);
                                //check for every 1000 mark
                                if (totalMark - tempMark >= 1000)
                                {
                                    currentLife++;
                                    tempMark += 1000;
                                }
                                //update number of bullet of the current ship
                                bg.Graphics.DrawString(UpdateBullet(), new Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Red), 10, 40);
                                //if ship run out of bullet, user loses the ship
                                if (bulletNumer == 0)
                                {
                                    currentLife--;
                                    //reset bullet number to the new ship
                                    bulletNumer = 8;
                                }
                                //Update the number of ship in the current play
                                bg.Graphics.DrawString(UpdateLife(), new Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Red), 10, 25);
                                //Giving instruction how to pause/resume the game
                                bg.Graphics.DrawString("Press U to pause/resume the game", new Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Red), 700, 15);
                                //remove all the death asteroid as well as bullet
                                _listAsteroid.RemoveAll(s => s.IsMarkedForDeath == true);
                                _bulletList.RemoveAll(s => s.IsMarkedForDeath == true);
                                //get thhe copy of the current asteroid list
                                tempAsteroidList = _listAsteroid.ToList();
                                //check fore intersection between asteroid and ship
                                foreach (Asteroid a in tempAsteroidList)
                                {
                                    //if they collapse each other
                                    if (userShip.Collapse(a, gr))
                                    {
                                        //current ship is destroyed
                                        currentLife -= 1;
                                        //a new ship added
                                        userShip = new Ship(new PointF(this.Width / 2, this.Height / 2));
                                        //a new bullet ship is created for the new ship
                                        _bulletList = new List<Bullet>();
                                        //bullet number is resey
                                        bulletNumer = 8;
                                        //delete the asteroid that collapse with the ship
                                        a.IsMarkedForDeath = true;
                                        break;
                                    }
                                }
                                //remove the deathship again
                                _listAsteroid.RemoveAll(s => s.IsMarkedForDeath == true);
                                //render all asteroid
                                lock (_listAsteroid)
                                    _listAsteroid.ForEach(s => s.Render(bg));
                                //update the movement of bullet, ship and asteroid
                                _bulletList.ForEach(s => s.Tick(ClientSize));
                                _bulletList.ForEach(s => s.Render(bg));
                                userShip.Tick(this.Size);
                                _listAsteroid.ForEach(s => s.Tick(this.Size));
                                bg.Render();
                            }
                        }

                        //if user lost
                        else
                        {
                            //clear buffer
                            bg.Graphics.Clear(Color.Black);
                            //show user's total points and display instruction to play again
                            bg.Graphics.DrawString($"You lost with {totalMark} points. Pressed Enter to play again!", new Font(FontFamily.GenericSansSerif, 20), new SolidBrush(Color.Red), 250, 450);
                            bg.Render();
                        }
                    }
                    else
                    {
                        bg.Graphics.Clear(Color.Black);
                        //show user's total points and display instruction to play again
                        bg.Graphics.DrawString(instruction, new Font(FontFamily.GenericSansSerif, 15), new SolidBrush(Color.Red), 250, 450);
                        bg.Render();
                    }
                }

            }
        }


     
        ////////////////////////////////////////////////////////
        /// Keydown events to check for controlling the game
        /////////////////////////////////////////////////////// 
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            //bullet is fired by space
            if (e.KeyCode == Keys.Space)
            {
                //a new bullet is add to bullet list when its fired
                _bulletList.Add(new Bullet(userShip.GetPath().PathPoints[0], userShip._fRotation));
                //the current number of bullet of the current ship decreases
                bulletNumer--;
            }
            //Up key to rotate ship in clock-wise direction
            if(e.KeyCode == Keys.Up)
            {
                userShip._fRotationIncrement = 5;
            }
            //Down key to rotate ship in anti-clockwise direction
            if (e.KeyCode == Keys.Down)
            {
               userShip._fRotationIncrement = -5;
            }
            //check enter to play again after user lostt
            if (e.KeyCode == Keys.Enter&&loser==true)
            {
                GameLoad();
            }
            if (e.KeyCode == Keys.Enter)
            {
                GameLoad();
                gameStart=true;
            }
            //U user to pause/resume the game
            if (e.KeyCode == Keys.U)
            {
                if (!Stop)
                    Stop = true;
                else
                    Stop = false;
            }
            
        }

        ////////////////////////////////////////////////////////
        /// Keyup events to reset the rotation values (after each rotate of the ship)
        /////////////////////////////////////////////////////// 
        private void Game_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                userShip._fRotationIncrement = 0;
            }
            if (e.KeyCode == Keys.Down)
            {
                userShip._fRotationIncrement = 0;
            }
        }

       
    }
}