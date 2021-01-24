using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                String[] scenePaths = new String[2];
                scenePaths[0] = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Candle");
                scenePaths[1] = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Plate");

                String[] sceneFileNames = new String[2];
                sceneFileNames[0] = "Candlestick.3ds";
                sceneFileNames[1] = "Plate.3ds";

                m_world = new World(scenePaths, sceneFileNames, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_world.Animation) return;
            switch (e.Key)
            {
                case Key.F5: this.Close(); break;
                case Key.T: m_world.RotationX -= 5.0f; break;
                case Key.G: m_world.RotationX += 5.0f; break;
                case Key.F: m_world.RotationY -= 5.0f; break;
                case Key.H: m_world.RotationY += 5.0f; break;
                case Key.A: m_world.PlateRotation -= 5.0f; break;
                case Key.D: m_world.PlateRotation += 5.0f; break;
                case Key.Multiply: m_world.PlateScale += 0.05f; break;
                case Key.Divide: m_world.PlateScale -= 0.05f; break;
                case Key.Z: m_world.CandleSpotDiffuse -= 0.05f; break;
                case Key.X: m_world.CandleSpotDiffuse += 0.05f; break;
                case Key.C: m_world.Animation = true; m_world.Animate(); break;
                case Key.Add: m_world.SceneDistance -= 150.0f; break;
                case Key.Subtract: m_world.SceneDistance += 150.0f; break;
                /*case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool) opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            String[] scenePaths = new String[1];
                            scenePaths[0] = Directory.GetParent(opfModel.FileName).ToString();

                            String[] sceneFileNames = new String[1];
                            sceneFileNames[0] = Path.GetFileName(opfModel.FileName);

                            World newWorld = new World(scenePaths, sceneFileNames, (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK );
                        }
                    }
                    break;*/
            }
        }
    }
}
