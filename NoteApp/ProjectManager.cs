using System;
using System.IO;
using Newtonsoft.Json;

namespace NoteApp
{
    /// <summary>
    /// Статический класс для сохранения и загрузки данных из файлов
    /// </summary>
    public static class ProjectManager
    {
        /// <summary>
        /// Путь сохранения файла с проектом по умолчанию   
        /// </summary>
        public static string DefaultFilePath { get; private set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            + "\\NoteApp\\NoteApp.notes";
        /// <summary>
        /// Сохраняет объект <see cref="Project"/> в файл 
        /// </summary>
        /// <param name="project">Проект для сериализации</param>
        /// <param name="path">Путь файла для сохранения/загрузки проекта</param>
        public static void SaveToFile(Project project, string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, project);
            }
        }

        /// <summary>
        /// Загружает объект "Project" из файла
        /// </summary>
        /// <returns>Возвращает данные из файла преобразуя в объект "Project"</returns>
        public static Project LoadFromFile(string path)
        {
            JsonSerializer serializer = new JsonSerializer();

            if (!File.Exists(path))
            {
                return new Project(); 
            }

            try
            {
                using (StreamReader sr = new StreamReader(path))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    return (Project)serializer.Deserialize<Project>(reader);
                }
            }
            catch (JsonSerializationException)
            {
                return new Project();
            }
        }
    }
}
