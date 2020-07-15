using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace AstroGame
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm mainForm = new MainForm();
            Game.Init(mainForm); // Создаем графический буфер для формы
            Game.Load(); // Создаем игровые объекты на форме

            mainForm.Show(); // Отображаем форму

            Game.Draw(); // Прорисовываем игровые элементы

            Application.Run(mainForm);
        }
    }
}
