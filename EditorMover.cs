using UnityEngine;

namespace DefaultNamespace 
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

        //todo comment: Что произойдёт, если _delay > _duration?
		//Если значение задержки записи (_delay) будет больше общей продолжительности работы скрипта
		//(_duration), то в список Records не будет добавлено ни одной записи.
        private float _delay = 0.5f;
		private float _duration = 5f;

		private void Start()
		{
            //todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
            //Поиск компонентов с помощью GetComponent<T>() — это ресурсоёмкая операция.
			//Вызывать её каждый кадр в методе Update() крайне неэффективно,
			//так как компонент PositionSaver прикреплён к тому же игровому объекту(gameObject) и никуда не исчезает после начала игры.
            _save = GetComponent<PositionSaver>();
			_save.Records.Clear();
		}

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}

            //todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            //Если бы мы использовали конструкцию _currentDelay -= Time.deltaTime, то после достижения нуля таймер начал бы уходить в отрицательные значения (-0.1, -0.2...).
			//При сбросе он бы получал значение -0.5 + 0.5 = 0, и следующая итерация происходила бы мгновенно.
			//Чтобы этого избежать, пришлось бы добавлять проверку if (_currentDelay < 0) _currentDelay = 0;.
			//Текущий подход (-=) является более простым и общепринятым способом реализации таких циклических таймеров.
            _currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
                    //todo comment: Для чего сохраняется значение игрового времени?
                    //Синхронизация данных. Если есть несколько объектов, которые записывают свои пути, сохранение абсолютного времени позволяет сопоставить их траектории и понять, где они находились относительно друг друга в конкретный момент времени.
                    //Воспроизведение движения(Replay System).Имея массив точек { позиция, время },
                    //можно легко воссоздать движение объекта.Можно интерполировать положение между двумя ближайшими по времени точками,
                    //чтобы получить плавную анимацию движения в любой момент времени,
                    //независимо от того,
                    //с какой реальной скоростью воспроизводится запись.
                    Time = Time.time,
				});
			}
		}
	}
}