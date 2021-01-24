// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using SharpGL.SceneGraph.Quadrics;
using SharpGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        private float m_plateRotation = 0.0f;
        private float m_plateScale = 1.0f;
        private float m_candleSpotDiffuse = 1.0f;
        private bool m_animation = false;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 700.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value < -20.0f ? m_xRotation : value>70.0f ? m_xRotation : value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        public float PlateRotation
        {
            get { return m_plateRotation; }
            set { m_plateRotation = value; }
        }

        public float PlateScale
        {
            get { return m_plateScale; }
            set { m_plateScale = value; }
        }

        public float CandleSpotDiffuse
        {
            get { return m_candleSpotDiffuse; }
            set { m_candleSpotDiffuse = value<0? 0.0f: value>1?1.0f:value; }
        }

        public bool Animation
        {
            get { return m_animation; }
            set { m_animation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public DispatcherTimer Timer { get; private set; }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String[] scenePath, String[] sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);
            SetupTex(gl);
            m_scene.LoadScene();
            m_scene.Initialize();
        }

        public void Animate()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(5);
            Timer.Tick += new EventHandler(UpdateAnimation);
            Timer.Start();
        }

        private int stage = 0;
        private int units = 0, short_path_unit_len = 150;
        private float pos_x = 0, pos_z = 0;

        private void UpdateAnimation(object sender, EventArgs e)
        {
            if (stage == 6)
            {
                m_animation = false;
                Timer.Stop();
                stage = -1;
                pos_x = 0; 
                pos_z = 0;
            }
            else
            {
                if (stage == 0 || stage == -1)
                {
                    if (units < short_path_unit_len)
                    {
                        units += 1;
                        pos_z = -units;
                    }
                    else
                    {
                        units = 0;
                        stage = 1;
                    }
                }
                else if (stage == 1)
                {
                    if (units < short_path_unit_len)
                    {
                        units += 1;
                        pos_x = -units;
                    }
                    else
                    {
                        units = 0;
                        stage = 2;
                    }
                }
                else if (stage == 2)
                {
                    if (units < 2* short_path_unit_len)
                    {
                        units += 1;
                        pos_z = -short_path_unit_len + units;
                    }
                    else
                    {
                        units = 0;
                        stage = 3;
                    }
                }
                else if (stage == 3)
                {
                    if (units < 2* short_path_unit_len)
                    {
                        units += 1;
                        pos_x = -short_path_unit_len + units;
                    }
                    else
                    {
                        units = 0;
                        stage = 4;
                    }
                }
                else if (stage == 4)
                {
                    if (units < short_path_unit_len)
                    {
                        units += 1;
                        pos_z = short_path_unit_len - units;
                    }
                    else
                    {
                        units = 0;
                        stage = 5;
                    }
                }
                else if (stage == 5)
                {
                    if (units < short_path_unit_len)
                    {
                        units += 1;
                        pos_x = short_path_unit_len - units;
                    }
                    else
                    {
                        units = 0;
                        stage = 6;
                    }
                }

            }
        }

        private void DefaultQUAD(OpenGL gl, int texId, bool parquet=false)
        {
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[texId]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            if(parquet)
                gl.Scale(3, 3, 3);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(400.0f, 400.0f, 0.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-400.0f, 400.0f, 0.0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-400.0f, -400.0f, 0.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(400.0f, -400.0f, 0.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Normal(0.0f, 0.0f, 1.0f);
            gl.End();
        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            //gl.Viewport(0, 0, m_width, m_height);

            SetupLighting(gl);

            //gl.MatrixMode(OpenGL.GL_MODELVIEW);   // selektuj ModelView Matrix
            gl.PushMatrix();
            gl.Translate(0.0f, 200.0f, -m_sceneDistance);
            //float[] lightposition = { 0.0f, 700.0f, 0.0f, 1.0f };
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, lightposition);
            //float[] lightdirection = { 0.0f, -1.0f, 0.0f };
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_DIRECTION, lightdirection);
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            //gl.Enable(OpenGL.GL_LIGHT0);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.PushMatrix();
            if (stage != -1)
            {
                gl.Translate(pos_x, 0.0f, pos_z);
            }
            gl.Translate(0.0f, -200.0f, 0.0f);
            gl.Rotate(m_plateRotation, 0.0f, 1.0f, 0.0f);
            gl.Translate(0.0f, -200.0f, 0.0f);
            gl.Scale(m_plateScale, m_plateScale, m_plateScale);
            pointLightCaster(gl);
            gl.Translate(0.0f, 200.0f, 0.0f);
            m_scene.Draw();
            gl.PopMatrix();

            //gl.LoadIdentity();
            gl.PushMatrix();
            gl.Color(0.5f, 0.0f, 0.1f);
            gl.Translate(0.0f, -410.0f, 0.0f);
            gl.Rotate(-90.0f, 1.0f, 0.0f, 0.0f);
            DefaultQUAD(gl,1, true);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.3f, 0.05f, 0.1f);
            gl.Translate(400.0f, -10.0f, 0.0f);
            gl.Rotate(-90.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl,0);
            gl.PopMatrix();

            /*gl.PushMatrix();
            gl.Color(0.3f, 0.05f, 0.1f);
            gl.Translate(-400.0f, -10.0f, 0.0f);
            gl.Rotate(-270.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl,1);
            gl.PopMatrix();*/

            gl.PushMatrix();
            gl.Color(0.3f, 0.05f, 0.1f);
            gl.Translate(-400.0f, -10.0f, 0.0f);
            gl.Rotate(-270.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl,0);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.8f, 1.0f, 0.3f);
            gl.Translate(0.0f, -10.0f, -400.0f);
            //gl.Rotate(-180.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl,0);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.8f, 1.0f, 0.3f);
            gl.Translate(0.0f, -10.0f, 400.0f);
            gl.Rotate(-180.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl,0);
            gl.PopMatrix();


            gl.PopMatrix();






            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            //gl.LoadIdentity();
            //gl.Ortho2D(m_width / 2, m_width, 0, m_height / 2);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.LoadIdentity();

            //gl.Color(0.0f, 191.0f, 255.0f);
            //gl.Translate(-4f, -2f, 0.0f);

            //gl.PushMatrix();
            //gl.Translate(-1.5f, -2f, 0f);
            //gl.Scale(0.5f, 0.5f, 0.5f);
            //gl.DrawText("Times New Roman", 5f, 1f, 0.1f, "Predmet: Racunarska grafika");
            //gl.PopMatrix();

            //Resize(gl, m_width, m_height);

            //gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Viewport(m_width / 5 * 4, 0, m_width / 5, m_height / 5);
            gl.PushMatrix();
            gl.Translate(2.0f, -3.5f, 0.0f);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            //gl.Ortho2D(-15.0f, 15.0f, -12.0f, 12.0f);
            gl.Ortho2D(0, 14, -8, 0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);


            gl.PushMatrix();
            gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Translate(1.5f, -4.0f, 0.0f);

            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "Predmet: Racunarska grafika");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "_________________________");
            gl.PopMatrix();
            //gl.Translate(-11.5f, -1.0f, 0.0f);
            gl.Translate(0.0f, -1.0f, 0.0f);
            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "Sk.god: 2020/2021");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "_______________");
            gl.PopMatrix();
            //gl.Translate(-6.370f, -1.0f, 0.0f);
            gl.Translate(0.0f, -1.0f, 0.0f);
            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "Ime: Igor");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "________");
            gl.PopMatrix();
            //gl.Translate(-5.55f, -1.0f, 0.0f);
            gl.Translate(0.0f, -1.0f, 0.0f);
            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "Prezime: Sikuljak");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "_______________");
            gl.PopMatrix();
            //gl.Translate(-6.3f, -1.10f, 0.0f);
            gl.Translate(0.0f, -1.0f, 0.0f);
            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "Sifra zad: PF1S18.2");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.DrawText3D("Times New Roman Bold", 12, 0.0f, 0.0f, "_________________");
            gl.PopMatrix();

            gl.PopMatrix();
            Resize(gl, m_width, m_height);
            //gl.Viewport(0, 0, m_width, m_height);
            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            //gl.LoadIdentity();
            //gl.Perspective(45f, (float)m_width / m_height, 1.0f, 20000f);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            //gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();




            //gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);
            //gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            //gl.LoadIdentity();			              // resetuj Projection Matrix
            //gl.Ortho2D(m_width / 2, m_width, 0, m_height / 2);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);   // selektuj ModelView Matrix
            //gl.LoadIdentity();                // resetuj ModelView Matrix
            //gl.PushMatrix();
            //gl.DrawText(10, 30, 0.0f, 1.0f, 0.0f, "Courier New", 12, "Perspektiva");
            //gl.PopMatrix();


            //gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            //gl.LoadIdentity();			              // resetuj Projection Matrix

            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.Ortho(m_width / 2, m_width, 80, m_height / 2, -1.0f, 1.0f);
            //gl.PushMatrix();
            //gl.Ortho(m_width / 2, m_width, 0, m_height / 2, -1.0f, 1.0f);
            //gl.DrawText(10, 30, 0.0f, 1.0f, 0.0f, "Courier New", 12, "Perspektiva");
            //gl.PopMatrix();
            //gl.Viewport(0, 0, m_width, m_height);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.LoadIdentity();
            //gl.Viewport(0, 0, m_width, m_height);
            //gl.PushMatrix();
            //gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            //gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            //gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            //m_scene.Draw();
            //gl.PopMatrix();

            //gl.PushMatrix();
            //gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);
            //gl.Ortho2D(m_width / 2, m_width, 0, m_height / 2);
            //gl.Ortho(m_width / 2, m_width, 80, m_height / 2, -1.0f, 1.0f);
            //gl.DrawText(10, 30, 0.0f, 1.0f, 0.0f, "Courier New", 12, "Perspektiva");
            //gl.PopMatrix();
            //gl.LoadIdentity();
            //gl.Viewport(0, 0, m_width, m_height);
            //gl.gl
            // Oznaci kraj iscrtavanja
            gl.Flush();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, width, height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 1.0f, 20000f);
            if(stage == -1 || stage == 0 || stage == 4 || stage == 6) gl.LookAt(pos_x, 500.0f, -m_sceneDistance/ 5.0f + pos_z, pos_x, 150.0f, -m_sceneDistance + pos_z, 0f, 1f, 0f);
            else if (stage == 1 || stage == 5) gl.LookAt(-m_sceneDistance / 5.0f - (-m_sceneDistance) + pos_z, 500.0f, -m_sceneDistance + pos_z, pos_x, 150.0f, -m_sceneDistance + pos_z, 0f, 1f, 0f);
            else if (stage == 2) gl.LookAt(pos_x, 500.0f, -2*m_sceneDistance+m_sceneDistance / 5.0f + pos_z, pos_x, 150.0f, -m_sceneDistance + pos_z, 0f, 1f, 0f);
            else if (stage == 3) gl.LookAt(-(-m_sceneDistance / 5.0f - (-m_sceneDistance)) + pos_z, 500.0f, -m_sceneDistance + pos_z, pos_x, 150.0f, -m_sceneDistance + pos_z, 0f, 1f, 0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode

        private void SetupLighting(OpenGL gl)
        {
            
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_NORMALIZE);
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            //pointLightCaster(gl);
            spotLightCaster(gl);
        }

        private void pointLightCaster(OpenGL gl)
        {
            
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, new float[] { 0.0f, 410.0f, 0f, 1.0f });
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, new float[] { m_candleSpotDiffuse, m_candleSpotDiffuse, m_candleSpotDiffuse, 1.0f });
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, new float[] { 1f, 0f, 0f, 1.0f });
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_DIRECTION, new float[] { 0.0f, 1.0f, 0.0f, 1.0f });
            //gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, PointSpecular);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 100.0f);
            gl.Enable(OpenGL.GL_LIGHT1);
            
        }

        private void spotLightCaster(OpenGL gl)
        {

            float[] ambijentalnaKomponenta = { 0.192f, 0.192f, 0.192f, 1.0f };//SpotlightAmbient;
            float[] difuznaKomponenta = { 0.507f, 0.507f, 0.507f, 1.0f };//SpotlightDiffuse;
            float[] spekularnaKomponenta = { 0.508f, 0.508f, 0.508f, 1.0f }; //SpotlightSpecular;
            float[] direction = { 0.0f, -1.0f, 0.0f, 1.0f};
            float[] position = { 0.0f, 700.0f, -m_sceneDistance, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, difuznaKomponenta);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, spekularnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SHININESS, 51.2f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, position);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_DIRECTION, direction);
            gl.Enable(OpenGL.GL_LIGHT0);


        }
        public static uint[] m_textures = new uint[2];
        private void SetupTex(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D); 
            gl.GenTextures(2, m_textures);
            for(int i = 0; i < 2; i++)
            {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);
                Bitmap image = null;
                if(i==0) image = new Bitmap("..//..//Tex//brick.png");
                else image = new Bitmap("..//..//Tex//parquet.jpg");
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
                image.UnlockBits(imageData);
                image.Dispose();
            }
            /*for (int i = 0; i < m_textureCount; ++i)
            {

                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                Bitmap image = new Bitmap(m_textureFiles[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_NEAREST);
                //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST_MIPMAP_NEAREST); //TODO: Sjebava tekst iz nekog razloga
                

                image.UnlockBits(imageData);
                image.Dispose();
            }*/
        }
    }
}
