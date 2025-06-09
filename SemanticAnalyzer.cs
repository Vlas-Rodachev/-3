namespace Компилятор
{
    public static class SemanticAnalyzer
    {
        // Таблица символов: имя -> тип
        public static Dictionary<string, string> SymbolTable = new Dictionary<string, string>();

        // Таблица массивов: имя -> (тип элементов, нижняя граница, верхняя граница)
        public static Dictionary<string, (string, int, int)> ArrayTable = new Dictionary<string, (string, int, int)>();

        // Флаги состояния
        private static bool _inAssignment; // Флаг, указывающий на обработку операции присваивания
        private static bool _inArrayDeclaration; // Флаг, указывающий на объявление массива
        private static string _currentArrayName; // Имя текущего объявляемого массива
        private static string _currentArrayType; // Тип элементов текущего массива

        // Начало обработки операции присваивания
        public static void StartAssignment()
        {
            _inAssignment = true;
        }

        // Завершение обработки операции присваивания
        public static void EndAssignment()
        {
            _inAssignment = false;
        }

        // Начало объявления массива
        public static void StartArrayDeclaration(string arrayName)
        {
            _inArrayDeclaration = true;
            _currentArrayName = arrayName;
        }

        // Завершение объявления массива
        public static void EndArrayDeclaration()
        {
            _inArrayDeclaration = false;
            _currentArrayName = null;
            _currentArrayType = null;
        }

        // Установка типа элементов массива
        public static void SetArrayType(string type)
        {
            _currentArrayType = type;
        }

        // Добавление информации о границах массива
        public static void AddArrayInfo(int lowerBound, int upperBound)
        {
            if (_inArrayDeclaration && _currentArrayName != null && _currentArrayType != null)
            {
                // Сохраняем информацию о массиве: тип элементов, нижняя и верхняя границы
                ArrayTable[_currentArrayName] = (_currentArrayType, lowerBound, upperBound);
            }
        }

        // Добавление переменной в таблицу символов
        public static void AddVariable(string name, string type)
        {
            if (SymbolTable.ContainsKey(name))
            {
                // Ошибка: повторное объявление переменной
                InputOutput.Error(200, InputOutput.positionNow);
            }
            else
            {
                SymbolTable.Add(name, type);
            }
        }

        // Проверка корректности операции присваивания
        public static void CheckAssignment(string leftVar, string expressionType)
        {
            if (!SymbolTable.ContainsKey(leftVar))
            {
                // Ошибка: переменная не объявлена
                InputOutput.Error(88, InputOutput.positionNow);
                return;
            }

            string leftType = SymbolTable[leftVar];

            // Проверка совместимости типов
            if (leftType != expressionType)
            {
                // Разрешено присваивание целого числа вещественной переменной
                if (!(leftType == "real" && expressionType == "integer"))
                {
                    // Ошибка: несовместимость типов
                    InputOutput.Error(201, InputOutput.positionNow);
                }
            }
        }

        // Проверка доступа к элементу массива
        public static void CheckArrayAccess(string arrayName, int index)
        {
            if (!ArrayTable.ContainsKey(arrayName))
            {
                // Ошибка: переменная не является массивом
                InputOutput.Error(202, InputOutput.positionNow);
                return;
            }

            var (_, lowerBound, upperBound) = ArrayTable[arrayName];

            // Проверка границ массива
            if (index < lowerBound || index > upperBound)
            {
                // Ошибка: выход за границы массива
                InputOutput.Error(203, InputOutput.positionNow);
            }
        }

        // Проверка бинарной операции
        public static string CheckBinaryOperation(string leftType, string rightType, string operation)
        {
            // Если один из типов содержит ошибку, возвращаем ошибку
            if (leftType == "error" || rightType == "error")
                return "error";

            // Проверка совместимости типов
            if (leftType != rightType)
            {
                // Разрешены операции между целыми и вещественными числами
                if ((leftType == "real" && rightType == "integer") ||
                    (leftType == "integer" && rightType == "real"))
                {
                    return "real";
                }
                else
                {
                    // Ошибка: несовместимость типов
                    InputOutput.Error(201, InputOutput.positionNow);
                    return "error";
                }
            }

            // Проверка допустимости операции для типов
            if (operation == "+" || operation == "-" || operation == "*" || operation == "/")
            {
                // Операции +-*/ разрешены только для числовых типов
                if (leftType == "integer" || leftType == "real")
                    return leftType;
                else
                {
                    // Ошибка: недопустимая операция для типа
                    InputOutput.Error(204, InputOutput.positionNow);
                    return "error";
                }
            }

            return leftType;
        }
    }
}