     В решении задачи используется библеотека Gnuplot. С помощью неё я только визуа-
лизирую получившиеся результаты. Библеотека не моя и находится в открытом доступе.
Ссылка: http://www.gnuplot.info
--------------------------------------------------------------------------------------
     К сожалению, как я понял, в моей задаче нельзя просто задать случайные данные
так как проверить в последствии ответ задачи не представляется для меня возможным.
Поэтому я задам конкретные данные и объясню, как они устроены и какой должен получи-
ться ответ.
     Эти рассуждения покажут по какому принципу в пространстве расположена область и
какие и почему координаты расположены на краях матрицы.
     Также в папке photos есть фотографии поверхности которая должна получиться,
поэтому если нужно можете не читать всё это и сразу посмотреть фотографии.

     Объяснение:
     Представляем в голове прямоугольник размерами w на h в пространстве.
     Я могу передвинуть область расчётов как мне удобно, потому что нет никакой 
привязки к координатам в изначальной матрице, поэтому будем считать, что matrix[0, 0]
это значение некой функции в точке (0, 0), а вся область лежит в плоскости образо-
ванной осями Ox и Oy, в первом координатном угле (т. е. все координаты по осям Оx и 
Оy не отрицательны).

     Задам данные. Задавать данные я буду прямыми (для простоты), чтобы я мог сверить
расчёты моей программы и мои собственные расчёты.
     
     Заполняю первую сторону. Пусть это будет сторона лежащая на оси Ox.
Длина этой стороны будет равна w (на самом деле это не длина, а частота узлов сетки, 
но в целом это не играет никакой роли, поэтому будем считать что это длина). Пусть 
эта сторона лежит на прямой, которая проходит через точки (0, 0, 0) и (w, 0, w), т.
е. в точке (w, 0) значение функции равно w. Тогда относительно осей Ox и Oz прямая, 
на которой лежит сторона области, является биссектрисой первого координатного угла,
образованного осями Ox и Oz.
     
     Заполняю вторую сторону. Пусть это будет сторона лежащая на оси Oy.
Длина этой стороны будет равна h. Пусть эта сторона лежит на прямой, которая прохо-
дит через точки (0, 0, 0) и (0, h, h), т.е. в точке (0, h) значение функции равно h.
Тогда относительно осей Oy и Oz прямая, на которой лежит сторона области, является 
биссектрисой первого координатного угла, образованного осями Oy и Oz.

     Заполняю третью, противоположную второй, сторону. Пусть эта сторона
также лежит на прямой. Эта прямая будет убывающая (чтобы итоговая поверхность полу-
чилась слишком простой) и также, как и предыдущие, будет параллельна биссектрисе. 
(пояснение: если сделать проекцию этой прямой на плоскость, образованную осями Oy и
Oz, то она будет параллельна биссектрисе 4-го координатного угла). Также пусть эта 
прямая проходит через точку (w, 0, w). Тогда эта прямая также проходит через точку 
(w, h, -w).

     Последнюю сторону строю, соединяя точки (w, h, -w) и (0, h, h) (концы 2-ой и 
3-ей сторон).

     Итоговая поверхность должна быть изгибающейся полосой.
     (фото, если я не забыл, будет лежать рядом с программой в папке Photos)

--------------------------------------------------------------------------------------

----------------------------------- 11.04.2023 ---------------------------------------

    Переделал под таски, вот список изенений и текущих проблем: 
    
    1) Упала производительность, предыдущая версия программы работала на точности 8,
эта же версия слишком долго работает. 
    В обеих версиях возможно проблема в расчётах, потому что странно что программа 
моментально расчитывает точность 7, но очень долго расчитывает точность 8 (для преды-
дущей версии то же самое для точностей 8 и девять). Вполне возможно, что каждый из 
Task-ов попадает в бесконечный цикл из-за неточности типа double.
    (Возможное решеение: переписать под decimal)
    
    (Новое!)

    2) Один Task может закончиться раньше другого, из-за, судя по всему, неправильной
функции _isAccuracy 
    (Возможное решение: попробовать проверять на правильность скопированный массив, а 
не основной).

--------------------------------------------------------------------------------------
----------------------------------- 24.04.2023 ---------------------------------------
    
    Всё работает
    - Перешёл на decimal, для бОльшей точности.
    - Нашёл крит. точку, из-за которой были баги (функция _isAccuracy)
    - Добавил токены отмены чтобы ничего не поломалось)

    В планах сделать кастомный ввод и визуализацию в gnuplot-е (только для windows)

--------------------------------------------------------------------------------------
----------------------------------- 25.04.2023 ---------------------------------------
    
    Визуализация есть, осталось кастомный ввод организовать.
    - Визуализация через gnuplot правда весит много (100 мегабайт)
    - Добавил пару методов: Сохранение результатов и запуск gnuplot-а по результатам

--------------------------------------------------------------------------------------
----------------------------------- 03.04.2023 ---------------------------------------

    Надо решить пару организационных моментов в LaplaceEquation
    - Организовать нормальный вывод и ввод и разобраться с слишком большим кол-вом
      полей в GridApproximator-е
    Визуализация удалена (возможно к ней вернусь)
    Добавлен MAUI интерфейс в отдельном проекте (не доделан)
    - Главная страница (готово)
    - Страница с настройками алгоритма (не доделано)
    - Страница с инструкцией (пока пустая)
    - Кнопка выхода

-------------------------------- Позже этот же день ----------------------------------

    LaplaceEquation:
    - Вывод ввод организовали но полей много, наверное ничего больше не сделаешь
    - Теперь класс статический

    UI:
    - К maui добавил страницу с результатами, страницу с настройками
    - Реализовал немного логики с данными
    - Заменил все "s" на "sender". Подумал что так логичнее 
      (s может быть сокращением от string)
    
    Другое:
    - Сделал nuget пакет из решения задачи, подключил его к MAUI интерфейсу

    TODO: 
    - Разбросаны по проекту, тут нет смысла перечислять 
      (по секрету там ещё вагон работы)
    - Надо решить проблему с сохранением конфига в файл, а то он не разрешает
      говорит пермиссион денаед(

--------------------------------------------------------------------------------------
----------------------------------- 04.04.2023 ---------------------------------------

    LaplaceEquation:
    - Теперь не статический, потому что статические классы не умеют наследоваться.
      (требуется по условию задачи)

    Другое:
    - Раскидал всё по папкам, создал два отдельных solution-а. 
      Теперь GUI - графический интерфейс, находящийся в одном solution-е с проектом
      ApproximatorInterface. Этот проект предоставляет программный интерфейс для 
      проекта с решением. Этот програмный интерфейс упаковывается в nuget пакет и 
      отправляется в папку NugetPackage. Оттуда его подключает решение задачи и
      использует как интерфейс для наследования.

    TODO:
    - (Не изменились)

--------------------------------------------------------------------------------------
----------------------------------- 08.04.2023 ---------------------------------------

    GUI:
    - Доделал GUI (полностью реализовал страницы которые хотел добавить).

    TODO:
    - (дополнительно) Добавить настройку с запуском Gnuplot-а как отдельного окна
    - Есть ощущение, что где-то есть баги. Но найти я их пока не смог. А в целом 
      вроде всё готово.
    - Можно попробовать понять где могут быть exception-ы и обернуть в try-catch, но 
      я не знаю что оборачивать.

--------------------------------------------------------------------------------------
----------------------------------- 16.04.2023 ---------------------------------------

   GUI:
   - Доделал GUI (ещё одна страница)
   - Исправил несколько ошибок из-за которых приложение могло крашнуться

--------------------------------------------------------------------------------------
