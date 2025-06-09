namespace Компилятор
{
    class SyntaxAnalis
    {
        // Список объявленных переменных
        private static List<string> variables = new List<string>();

        // Текущий символ
        private static byte symbol;

        // Допустимые типы данных
        private static string[] types = { "integer", "real", "char" };

        // Флаги состояния
        public static bool be_end_flag, begino, endp = false;

        // Лексический анализатор
        private static LexicalAnalyzer lex = new LexicalAnalyzer();

        // Флаги и переменные для обработки массивов
        private static bool _isArrayDeclaration = false;
        private static string _currentArrayName;
        private static int _arrayLowerBound, _arrayUpperBound;

        // Проверка корректности текущего символа
        private static bool Correct(byte symbol_Correct, TextPosition TextPos)
        {
            if (symbol == symbol_Correct)
            {
                return true;
            }
            else
            {
                // Вывод ошибки, если символ не соответствует ожидаемому
                InputOutput.Error(symbol_Correct, TextPos);
                return false;
            }
        }

        // Проверка корректности типа данных
        public static void chechType()
        {
            if (symbol == LexicalAnalyzer.ident)
            {
                string type = lex.ident_name;
                bool q = false;
                // Проверяем, является ли тип допустимым
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i] == type)
                    {
                        q = true;
                    }
                }
                if (q == false)
                {
                    // Удаляем последнюю добавленную переменную, если тип неверный
                    if (variables.Count != 0)
                    {
                        variables.RemoveAt(variables.Count - 1);
                    }
                    // Ошибка: несуществующий тип
                    InputOutput.Error(94, InputOutput.positionNow);
                }
                else if (_isArrayDeclaration)
                {
                    // Устанавливаем тип для массива
                    SemanticAnalyzer.SetArrayType(type);
                }
            }
        }

        // Основной метод синтаксического анализа
        public static void SyntaxAlanyzer()
        {
            // Получаем следующий символ, если текущий не begin или end
            if (symbol != LexicalAnalyzer.beginsy && symbol != LexicalAnalyzer.endsy)
            {
                symbol = lex.NextSym();
            }

            // Обработка различных случаев в зависимости от текущего символа
            switch (symbol)
            {
                case LexicalAnalyzer.programsy:
                    Program();
                    break;
                case LexicalAnalyzer.varsy:
                    if (!be_end_flag)
                    {
                        Varvar();
                    }
                    break;
                case LexicalAnalyzer.beginsy:
                    begino = true;
                    Begin();
                    break;
                case LexicalAnalyzer.ident:
                    Ident();
                    break;
                case LexicalAnalyzer.endsy:
                    End();
                    break;
            }
        }

        // Обработка раздела объявления переменных
        public static void Varvar()
        {
            if (symbol == LexicalAnalyzer.varsy)
            {
                symbol = lex.NextSym();
                do
                {
                    // Обработка объявления переменных
                    Vardeclaration();
                    symbol = lex.NextSym();
                    // Проверка точки с запятой
                    Correct(LexicalAnalyzer.semicolon, InputOutput.positionNow);
                    symbol = lex.NextSym();
                }
                while (symbol == LexicalAnalyzer.ident);
                SyntaxAlanyzer();
            }
        }

        // Обработка объявления переменных
        public static void Vardeclaration()
        {
            int countVariables = 1;
            // Проверка идентификатора
            Correct(LexicalAnalyzer.ident, InputOutput.positionNow);
            variables.Add(lex.ident_name);
            // Добавление переменной в таблицу символов с временным типом "unknown"
            SemanticAnalyzer.AddVariable(lex.ident_name, "unknown");
            symbol = lex.NextSym();

            // Обработка списка переменных через запятую
            while (symbol == LexicalAnalyzer.comma)
            {
                countVariables++;
                symbol = lex.NextSym();
                Correct(LexicalAnalyzer.ident, InputOutput.positionNow);
                variables.Add(lex.ident_name);
                SemanticAnalyzer.AddVariable(lex.ident_name, "unknown");
                symbol = lex.NextSym();
            }

            // Проверка двоеточия
            Correct(LexicalAnalyzer.colon, InputOutput.positionNow);
            symbol = lex.NextSym();

            // Обработка массива или простого типа
            if (symbol == LexicalAnalyzer.arraysy)
            {
                Array();
            }
            else
            {
                chechType();
                // Обновление типа в таблице символов
                if (variables.Count > 0 && symbol == LexicalAnalyzer.ident)
                {
                    for (int i = variables.Count - countVariables; i < variables.Count; i++)
                    {
                        if (SemanticAnalyzer.SymbolTable[variables[i]] == "unknown")
                        {
                            SemanticAnalyzer.SymbolTable[variables[i]] = lex.ident_name;
                        }
                    }
                }
            }
        }

        // Обработка блока begin
        public static void Begin()
        {
            Correct(LexicalAnalyzer.beginsy, InputOutput.positionNow);
            symbol = lex.NextSym();
            SyntaxAlanyzer();
        }

        // Обработка блока end
        public static void End()
        {
            if (begino)
            {
                Correct(LexicalAnalyzer.endsy, InputOutput.positionNow);
                symbol = lex.NextSym();
                endp = true;
                Correct(LexicalAnalyzer.point, InputOutput.positionNow);
            }
            else
            {
                // Ошибка: отсутствует парный begin
                InputOutput.Error(113, InputOutput.positionNow);
                Console.WriteLine("**" + InputOutput.errCount + "**" + "^ ошибка код 113");
                Console.WriteLine("Ожидался парный begin");
            }
        }

        // Обработка идентификатора
        public static void Ident()
        {
            if (!Keywords.Kw.TryGetValue(lex.ident_name, out byte value))
            {
                if (InputOutput.startstring)
                {
                    if (variables.Contains(lex.ident_name))
                    {
                        string varName = lex.ident_name;
                        symbol = lex.NextSym();

                        // Обработка доступа к элементу массива
                        if (symbol == LexicalAnalyzer.lbracket)
                        {
                            symbol = lex.NextSym();

                            // Обработка индекса массива
                            if (symbol == LexicalAnalyzer.intc)
                            {
                                int index = lex.nmb_int;
                                // Семантическая проверка доступа к массиву
                                SemanticAnalyzer.CheckArrayAccess(varName, index);
                                symbol = lex.NextSym();
                            }
                            else if (symbol == LexicalAnalyzer.ident)
                            {
                                // Проверка существования переменной-индекса
                                if (!SemanticAnalyzer.SymbolTable.ContainsKey(lex.ident_name))
                                {
                                    InputOutput.Error(88, InputOutput.positionNow);
                                }
                                symbol = lex.NextSym();
                            }
                            else
                            {
                                InputOutput.Error(11, InputOutput.positionNow);
                            }

                            Correct(LexicalAnalyzer.rbracket, InputOutput.positionNow);
                            symbol = lex.NextSym();
                        }

                        // Обработка операции присваивания
                        if (symbol == LexicalAnalyzer.assign)
                        {
                            SemanticAnalyzer.StartAssignment();
                            symbol = lex.NextSym();

                            // Обработка правой части выражения
                            string exprType = ProcessExpression();
                            // Семантическая проверка присваивания
                            SemanticAnalyzer.CheckAssignment(varName, exprType);
                            SemanticAnalyzer.EndAssignment();
                        }
                    }
                    else
                    {
                        // Ошибка: переменная не объявлена
                        InputOutput.Error(88, InputOutput.positionNow);
                    }
                }
            }
        }

        // Обработка объявления массива
        public static void Array()
        {
            Correct(LexicalAnalyzer.arraysy, InputOutput.positionNow);
            _isArrayDeclaration = true;
            _currentArrayName = variables[variables.Count - 1];
            // Начало объявления массива
            SemanticAnalyzer.StartArrayDeclaration(_currentArrayName);

            symbol = lex.NextSym();
            Correct(LexicalAnalyzer.lbracket, InputOutput.positionNow);
            symbol = lex.NextSym();

            // Обработка нижней границы массива
            if (symbol == LexicalAnalyzer.intc)
            {
                _arrayLowerBound = lex.nmb_int;
                symbol = lex.NextSym();

                if (symbol == LexicalAnalyzer.rbracket)
                {
                    // Массив с одним элементом
                    _arrayUpperBound = _arrayLowerBound;
                    symbol = lex.NextSym();
                    Correct(LexicalAnalyzer.ofsy, InputOutput.positionNow);
                    symbol = lex.NextSym();
                    chechType();

                    // Добавление информации о массиве
                    SemanticAnalyzer.AddArrayInfo(_arrayLowerBound, _arrayUpperBound);
                    _isArrayDeclaration = false;
                    SemanticAnalyzer.EndArrayDeclaration();

                    // Обновление типа массива в таблице символов
                    if (variables.Count > 0)
                    {
                        SemanticAnalyzer.SymbolTable[_currentArrayName] = lex.ident_name;
                    }
                }
                else
                {
                    Correct(LexicalAnalyzer.twopoints, InputOutput.positionNow);
                    symbol = lex.NextSym();

                    // Обработка верхней границы массива
                    if (symbol == LexicalAnalyzer.intc)
                    {
                        _arrayUpperBound = lex.nmb_int;

                        symbol = lex.NextSym();
                        Correct(LexicalAnalyzer.rbracket, InputOutput.positionNow);

                        symbol = lex.NextSym();
                        Correct(LexicalAnalyzer.ofsy, InputOutput.positionNow);
                        symbol = lex.NextSym();
                        chechType();

                        // Добавление информации о массиве
                        SemanticAnalyzer.AddArrayInfo(_arrayLowerBound, _arrayUpperBound);

                        _isArrayDeclaration = false;
                        SemanticAnalyzer.EndArrayDeclaration();

                        // Обновление типа массива в таблице символов
                        if (variables.Count > 0)
                        {
                            SemanticAnalyzer.SymbolTable[_currentArrayName] = lex.ident_name;
                        }
                    }
                    else
                    {
                        // Ошибка: неверный размер массива
                        InputOutput.Error(1, InputOutput.positionNow);
                        Console.WriteLine("**" + InputOutput.errCount + "**" + "^ ошибка код 1");
                        Console.WriteLine("Неверный размер массива");
                    }
                }
            }
            else
            {
                // Ошибка: неверный размер массива
                InputOutput.Error(1, InputOutput.positionNow);
                Console.WriteLine("**" + InputOutput.errCount + "**" + "^ ошибка код 1");
                Console.WriteLine("Неверный размер массива");
            }
        }

        // Обработка заголовка программы
        public static void Program()
        {
            Correct(LexicalAnalyzer.programsy, InputOutput.positionNow);
            symbol = lex.NextSym();
            Correct(LexicalAnalyzer.ident, InputOutput.positionNow);
            symbol = lex.NextSym();
            Correct(LexicalAnalyzer.semicolon, InputOutput.positionNow);
            symbol = lex.NextSym();
        }

        // Обработка выражения
        private static string ProcessExpression()
        {
            string typeLeft = ProcessTerm();

            // Обработка операций + и -
            while (symbol == LexicalAnalyzer.plus || symbol == LexicalAnalyzer.minus)
            {
                byte op = symbol;
                symbol = lex.NextSym();
                string typeRight = ProcessTerm();
                string opStr = op == LexicalAnalyzer.plus ? "+" : "-";
                // Семантическая проверка бинарной операции
                typeLeft = SemanticAnalyzer.CheckBinaryOperation(typeLeft, typeRight, opStr);
            }

            return typeLeft;
        }

        // Обработка терма
        private static string ProcessTerm()
        {
            string typeLeft = ProcessFactor();

            // Обработка операций * и /
            while (symbol == LexicalAnalyzer.star || symbol == LexicalAnalyzer.slash)
            {
                byte op = symbol;
                symbol = lex.NextSym();
                string typeRight = ProcessFactor();
                string opStr = op == LexicalAnalyzer.star ? "*" : "/";
                // Семантическая проверка бинарной операции
                typeLeft = SemanticAnalyzer.CheckBinaryOperation(typeLeft, typeRight, opStr);
            }

            return typeLeft;
        }

        private static string ProcessFactor()
        {
            if (symbol == LexicalAnalyzer.ident)
            {
                string varName = lex.ident_name;
                if (!SemanticAnalyzer.SymbolTable.ContainsKey(varName))
                {
                    // Ошибка: переменная не объявлена
                    InputOutput.Error(88, InputOutput.positionNow);
                    symbol = lex.NextSym();
                    return "error";
                }

                string type = SemanticAnalyzer.SymbolTable[varName];
                symbol = lex.NextSym();

                // Обработка доступа к элементу массива
                if (symbol == LexicalAnalyzer.lbracket)
                {
                    symbol = lex.NextSym();
                    if (symbol == LexicalAnalyzer.intc)
                    {
                        SemanticAnalyzer.CheckArrayAccess(varName, lex.nmb_int);
                    }
                    string indexType = ProcessExpression();

                    // Проверка типа индекса (должен быть integer)
                    if (indexType != "integer")
                        InputOutput.Error(204, InputOutput.positionNow);

                    Correct(LexicalAnalyzer.rbracket, InputOutput.positionNow);
                    symbol = lex.NextSym();

                    // Получение типа элемента массива
                    if (SemanticAnalyzer.ArrayTable.ContainsKey(varName))
                        type = SemanticAnalyzer.ArrayTable[varName].Item1;
                    else
                    {
                        // Ошибка: попытка индексации не-массива
                        InputOutput.Error(202, InputOutput.positionNow);
                        type = "error";
                    }
                }
                return type;
            }
            else if (symbol == LexicalAnalyzer.intc)
            {
                // Целочисленная константа
                symbol = lex.NextSym();
                return "integer";
            }
            else if (symbol == LexicalAnalyzer.leftpar)
            {
                // Обработка выражения в скобках
                symbol = lex.NextSym();
                string type = ProcessExpression();
                Correct(LexicalAnalyzer.rightpar, InputOutput.positionNow);
                symbol = lex.NextSym();
                return type;
            }
            else
            {
                // Ошибка: неверный символ
                InputOutput.Error(6, InputOutput.positionNow);
                symbol = lex.NextSym();
                return "error";
            }
        }
    }
}