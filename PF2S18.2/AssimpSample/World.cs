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
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
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
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            m_scene.LoadScene();
            m_scene.Initialize();
        }


        private void DefaultQUAD(OpenGL gl)
        {
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(400.0f, 400.0f, 0.0f);
            gl.Vertex(-400.0f, 400.0f, 0.0f);
            gl.Vertex(-400.0f, -400.0f, 0.0f);
            gl.Vertex(400.0f, -400.0f, 0.0f);
            gl.End();
        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //gl.Viewport(0, 0, m_width, m_height);



            //gl.MatrixMode(OpenGL.GL_MODELVIEW);   // selektuj ModelView Matrix
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            m_scene.Draw();
            //gl.LoadIdentity();
            gl.PushMatrix();
            gl.Color(0.5f, 0.0f, 0.1f);
            gl.Translate(0.0f, -410.0f, 0.0f);
            gl.Rotate(-90.0f, 1.0f, 0.0f, 0.0f);
            DefaultQUAD(gl);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.3f, 0.05f, 0.1f);
            gl.Translate(400.0f, -10.0f, 0.0f);
            gl.Rotate(-90.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.3f, 0.05f, 0.1f);
            gl.Translate(-400.0f, -10.0f, 0.0f);
            gl.Rotate(-270.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.3f, 0.05f, 0.1f);
            gl.Translate(-400.0f, -10.0f, 0.0f);
            gl.Rotate(-270.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.8f, 1.0f, 0.3f);
            gl.Translate(0.0f, -10.0f, -400.0f);
            //gl.Rotate(-180.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.8f, 1.0f, 0.3f);
            gl.Translate(0.0f, -10.0f, 400.0f);
            gl.Rotate(-180.0f, 0.0f, 1.0f, 0.0f);
            DefaultQUAD(gl);
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
    }
}
