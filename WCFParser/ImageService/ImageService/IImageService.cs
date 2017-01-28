using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ImageService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IImageService" в коде и файле конфигурации.
    [ServiceContract]
    public interface IImageService
    {
        [OperationContract]
        void SetWord(); //принимает на вход экземпляр класса Word и записывает его в XML документ

        [OperationContract]
        Word GetWord(string word, string key); //принимает на вход ключ, определяющий тип поиска слова и возвращает экземпляр Word
    }
}
