using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace TVProgViewer.CryptoService
{
    public class PBKDF2: ICryptoService
    {
        public PBKDF2()
        {
            HashIterations = 103000;
            SaltSize = 48;
        }
        /// <summary>
        /// Получает/устанавливает итерации хеширования
        /// </summary>
        public int HashIterations { get; set; }

        /// <summary>
        /// Получает/устанавливает  размер соли, которая будет генериться, в случае, если Salt не установлена
        /// </summary>
        public int SaltSize { get; set; }

        /// <summary>
        /// Получает/устанавливает текст для хеширования
        /// </summary>
        public string PlainText { get; set; }

        /// <summary>
        /// Получить захешированный текст
        /// </summary>
        public string HashedText { get; private set; }

        /// <summary>
        /// Получает/уставнавливает соль, которая будет использоваться для вычсиления HashedText. Это поле содержит как саму Salt, так и HashIterations 
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Вычислить хеш
        /// </summary>
        /// <returns>Вычисленный хеш: HashedText</returns>
        public string Compute()
        {
            if (string.IsNullOrEmpty(PlainText)) throw new InvalidOperationException("PlainText не может быть пустым");
            
            // Если нет соли, генерируем её:
            if (string.IsNullOrEmpty(Salt))
                GenerateSalt();

            HashedText = calculateHash(HashIterations);
            
            return HashedText;
        }

        /// <summary>
        /// Вычислить хеш, использовав сгенерённую по умолчанию соль. Будет генериться соль, если она не была заранее привязана.
        /// </summary>
        /// <returns></returns>
        public  string Compute(string textToHash)
        {
            PlainText = textToHash;
            // Вычислить хеш:
            Compute();
            return HashedText;
        }

        /// <summary>
        /// Вычислить хеш, который будет также генериться с указанными солёными параметрами
        /// </summary>
        /// <param name="textToHash">текст для хеширования</param>
        /// <param name="saltSize">размер соли для генерации</param>
        /// <param name="hashIterations"></param>
        /// <returns>вычисленный хеш: HashedText</returns>
        public string Compute(string textToHash, int saltSize, int hashIterations)
        {
            PlainText = textToHash;
            // Сгенерить соль:
            GenerateSalt(hashIterations, saltSize);
            // Вычислить хеш:
            Compute();
            return HashedText;
        }

        /// <summary>
        /// Вычисление хеша, в котором будет использована подтвержденная соль 
        /// </summary>
        /// <param name="textToHash">текст для хеширования</param>
        /// <param name="salt">соль, которая задействована в вычислениях</param>
        /// <returns>вычисленный хеш: HashedText</returns>
        public string Compute(string textToHash, string salt)
        {
            PlainText = textToHash;
            Salt = salt;
            // Расширить соль:
            expandSalt();
            Compute();
            return HashedText;
        }

        /// <summary>
        /// Генерирование соли с размером и количеством итераций по умолчанию
        /// </summary>
        /// <returns>сгенерённая соль</returns>
        public string GenerateSalt()
        {
            if (SaltSize < 1) throw 
                new InvalidOperationException(string.Format("Невозоможно сгенерить соль размером {0}, используйте значение больше, чем 1, рекомендуется: 16", SaltSize));
            var rand = RandomNumberGenerator.Create();

            var ret = new byte[SaltSize];

            rand.GetBytes(ret);

            // Связать сгенерённую соль с форматом {итерации}.{соль}:
            Salt = string.Format("{0}.{1}", HashIterations, Convert.ToBase64String(ret));

            return Salt;
        }

        /// <summary>
        /// Генерирование соли
        /// </summary>
        /// <param name="hashIterations">количество итераций добавления соли</param>
        /// <param name="saltSize">размер соли</param>
        /// <returns></returns>
        public string GenerateSalt(int hashIterations, int saltSize)
        {
            HashIterations = hashIterations;
            SaltSize = saltSize;
            return GenerateSalt();
        }

        /// <summary>
        /// Получить время в миллисекундах которое истекло до полного завершения генерации хеша для итераций
        /// </summary>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public int GetElapsedTimeForIteration(int iteration)
        {
            var sw = new Stopwatch();
            sw.Start();
            calculateHash(iteration);
            return (int) sw.ElapsedMilliseconds;
        }

        private string calculateHash(int iteration)
        {
            // Перевести соль в массив байтов:
            byte[] saltBytes = Encoding.UTF8.GetBytes(Salt);

            var pbkdf2 = new Rfc2898DeriveBytes(PlainText, saltBytes, iteration);
            var key = pbkdf2.GetBytes(64);
            return Convert.ToBase64String(key);
        }

        private void expandSalt()
        {
            try
            {
                //Получить позицию точки, которая разобъет строки:
                var i = Salt.IndexOf('.');

                //Получить итерации хеша из первого индекса:
                HashIterations = int.Parse(Salt.Substring(0, i), System.Globalization.NumberStyles.Number);
            }
            catch(Exception)
            {
                throw new FormatException("Соль не была в ожидаемом формате {int}.{string}");
            }
        }
        /// <summary>
        /// Сравнить хеши на тождественность
        /// </summary>
        /// <param name="passwordHash1">Первый хеш пароля для сравнения</param>
        /// <param name="passwordHash2">Второй хеш пароля для сравнения</param>
        /// <returns>true: обозначает что хеши паролей одинаковы, иначе обратное.</returns>
        public bool Compare(string passwordHash1, string passwordHash2)
        {
            if (passwordHash1 == null || passwordHash2 == null)
                return false;

            int min_length = Math.Min(passwordHash1.Length, passwordHash2.Length);
            int result = 0;

            for (int i = 0; i < min_length; i++)
            {
                result |= passwordHash1[i] ^ passwordHash2[i];
            }

            return result == 0;
        }
    }
}
