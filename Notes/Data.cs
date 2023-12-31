﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes
{
    internal class Data
    {
        string fileName; // путь к базе

        public Data(string file) 
        {
            string basesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

            if (!Directory.Exists(basesDirectory))
                Directory.CreateDirectory(basesDirectory);

            fileName = Path.Combine(basesDirectory, file);

            if (!File.Exists(fileName))
                File.Create(fileName).Close();
        }

        /// <summary>
        /// Добавление элемента в текст
        /// </summary>
        /// <param name="elem"></param>
        public void Add(string elem) => File.AppendAllText(fileName, elem);

        /// <summary>
        /// Получить весь текст из файла
        /// </summary>
        /// <returns>Информация из базы ввиде массива строк</returns>
        public string GetAllText() => File.ReadAllText(fileName);

        /// <summary>
        /// Очистка текста
        /// </summary>
        public void Clear() => File.Create(fileName).Close();
    }
}
