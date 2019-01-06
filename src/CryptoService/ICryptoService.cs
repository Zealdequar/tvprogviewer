
namespace TVProgViewer.CryptoService
{
    interface ICryptoService
    {
        /// <summary>
        /// Получает/устанавливает итерации хеширования
        /// </summary>
        int HashIterations { get; set; }

        /// <summary>
        /// Получает/устанавливает размер соли, которая будет генериться, в случае, если Salt не установлена
        /// </summary>
        int SaltSize { get; set; }

        /// <summary>
        /// Получает/устанавливает текст для хеширования
        /// </summary>
        string PlainText { get; set; }

        /// <summary>
        /// Получить захешированный текст
        /// </summary>
        string HashedText { get; }

        /// <summary>
        /// Получает/уставнавливает соль, которая будет использоваться для вычсиления HashedText. Это поле содержит как саму Salt, так и HashIterations 
        /// </summary>
        string Salt { get; set; }

        /// <summary>
        /// Вычислить хеш
        /// </summary>
        /// <returns>Вычисленный хеш: HashedText</returns>
        string Compute();

        /// <summary>
        /// Вычислить хеш, использовав сгенерённую по умолчанию соль. Будет генериться соль, если она не была заранее привязана.
        /// </summary>
        /// <returns></returns>
        string Compute(string textToHash);
        
        /// <summary>
        /// Вычислить хеш, который будет также генериться с указанными солёными параметрами
        /// </summary>
        /// <param name="textToHash">текст для хеширования</param>
        /// <param name="saltSize">размер соли для генерации</param>
        /// <param name="hashIterations"></param>
        /// <returns>вычисленный хеш: HashedText</returns>
        string Compute(string textToHash, int saltSize, int hashIterations);

        /// <summary>
        /// Вычисление хеша, в котором будет использована подтвержденная соль 
        /// </summary>
        /// <param name="textToHash">текст для хеширования</param>
        /// <param name="salt">соль, которая задействована в вычислениях</param>
        /// <returns>вычисленный хеш: HashedText</returns>
        string Compute(string textToHash, string salt);

        /// <summary>
        /// Генерирование соли с размером и количеством итераций по умолчанию
        /// </summary>
        /// <returns>сгенерённая соль</returns>
        string GenerateSalt();

        /// <summary>
        /// Генерирование соли
        /// </summary>
        /// <param name="hashIterations">количество итераций добавления соли</param>
        /// <param name="saltSize">размер соли</param>
        /// <returns></returns>
        string GenerateSalt(int hashIterations, int saltSize);

        /// <summary>
        /// Получить время в миллисекундах которое истекло до полного завершения генерации хеша для итераций
        /// </summary>
        /// <param name="iteration"></param>
        /// <returns></returns>
        int GetElapsedTimeForIteration(int iteration);

        /// <summary>
        /// Сравнить хеши на тождественность
        /// </summary>
        /// <param name="passwordHash1">Первый хеш пароля для сравнения</param>
        /// <param name="passwordHash2">Второй хеш пароля для сравнения</param>
        /// <returns>true: обозначает что хеши паролей одинаковы, иначе обратное.</returns>
        bool Compare(string passwordHash1, string passwordHash2);
    }
}
