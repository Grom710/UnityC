using System;
using System.Collections.Generic; 
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		public struct Data
		{
			public Vector3 Position;
			public float Time;
        }
        [SerializeField]
        private TextAsset _json;

        [SerializeField]
        [HideInInspector]
        public List<Data> Records { get; private set; }

		private void Awake()
		{
            //todo comment: Что будет, если в теле этого условия не сделать выход из метода? 
            //Если убрать оператор return;, то после вывода ошибки и отключения объекта (gameObject.SetActive(false)) выполнение метода Awake() продолжится.
            //В следующей строке кода будет попытка выполнить десериализацию:
            //JsonUtility.FromJsonOverwrite(_json.text, this);
            //Поскольку условие _json == null истинно, переменная _json содержит null. Попытка обратиться к свойству _json.text вызовет ошибку NullReferenceException.
            //Программа "упадёт", и в консоли Unity появится сообщение об этой ошибке, что помешает нормальной работе редактора.
            //Оператор return необходим, чтобы прервать выполнение метода и предотвратить эту ошибку.
            if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			JsonUtility.FromJsonOverwrite(_json.text, this);
            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)? 
            //Эта проверка служит для обеспечения отказоустойчивости (defensive programming).
            //JsonUtility.FromJsonOverwrite заполняет существующие поля объекта данными из JSON.
            //Если поле Records в JSON-файле имеет значение null или поле отсутствует, то после десериализации переменная Records в скрипте останется null.
            //Если не сделать эту проверку, то при попытке обратиться к Records возникнет ошибка NullReferenceException.
			//Эта строка кода гарантирует, что у объекта всегда будет корректный, пусть и пустой, список для работы, что позволяет избежать краха программы.
            if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
            //todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
            //Защита от NullReferenceException: Как и в предыдущем пункте, она предотвращает ошибку, если список Records по какой-то причине не был инициализирован.
            //Оптимизация: Если список пуст (Count == 0), то отрисовывать нечего. Выполнение метода на этом прекращается (return),
			//что экономит ресурсы процессора, так как отрисовка гизмо — это тоже вычислительная задача.

            if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
            //todo comment: Почему итерация начинается не с нулевого элемента?
            //Если бы цикл начинался с i = 0, то на первой итерации он попытался бы нарисовать линию от data[0] к data[0], что является точкой, а не линией.
			//Начиная с i = 1, мы сразу рисуем линию от первой точки ко второй, затем от второй к третьей и так далее.
			//Это позволяет нарисовать непрерывную траекторию из (n-1) отрезков для n точек.
            for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			//todo comment: Что происходит в этой строке?
			//////Создаёт файл: Метод File.Create создаёт новый файл по указанному пути. 
			//////Если файл уже существует, он будет перезаписан (его содержимое будет удалено).
			//////Возвращает поток: Метод возвращает объект FileStream, который представляет собой "канал" для чтения из или записи в этот файл.
			//////Сохраняет ссылку: Ссылка на этот поток сохраняется в переменной stream.
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
			//////Эта строка закрывает файл.
			//////В .NET есть правило: пока файл открыт одним процессом, другие процессы не могут его заблокировать для записи.
			//////Если закомментировать эту строку, то файл "Path.txt" останется открытым. 
			//////При попытке сохранить изменения в проекте Unity может выдать ошибку "Unable to save... Path.txt is being used by another process".
			//////Вызов Dispose() немедленно освобождает файл, позволяя Unity работать с ним дальше.
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				//////asset != null: Это стандартная защита от ошибок. 
				//////Хотя метод LoadAssetAtPath обычно возвращает объект, эта проверка делает код более надёжным.
				//////asset.name == "Path": Это ключевое условие поиска. Метод 
				//////FindAssets("t:TextAsset") находит все текстовые ассеты в проекте.
				//////Цикл перебирает их один за другим.
				//////Эта проверка позволяет найти именно тот ассет, который нам нужен — с именем "Path".
				//////Без неё скрипт попытался бы использовать первый же найденный текстовый файл, что привело бы к неверной работе.
				//////if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					//Как только мы нашли нужный нам ассет с именем "Path", наша задача выполнена. 
					//Мы присваиваем его переменной _json, помечаем текущий компонент как изменённый (SetDirty) и сохраняем проект.
					//Дальнейший перебор остальных ассетов не имеет смысла и является пустой тратой ресурсов процессора. 
					//Оператор return немедленно прерывает выполнение метода, как только цель достигнута.
					return;
				}
			}
		}

		private void OnDestroy()
		{
            if (!UnityEditor.EditorApplication.isPlaying || _json == null || Records == null)
                return;

            try
            {
                var wrapper = new Wrapper { dataList = Records };
                
                string json = JsonUtility.ToJson(wrapper, true);
                
                File.WriteAllText(UnityEditor.AssetDatabase.GetAssetPath(_json), json);
                
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Не удалось сохранить записи: {e.Message}", this);
            }
        }

        // Вспомогательный класс для сериализации списка данных.
        [Serializable]
        private class Wrapper
        {
            public List<Data> dataList;
        }
#endif
    }
}