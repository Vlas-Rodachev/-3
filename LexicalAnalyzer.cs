namespace Компилятор
{
    class LexicalAnalyzer
    {
        public const byte
            ident = 2,	                // идентификатор
            rightpar = 4,	            // )
            colon = 5,                  // :
            leftpar = 9,	            // (

            lbracket = 11,	            // [
            rbracket = 12,	            // ]
            semicolon = 14,             // ;
            intc = 15,	                // целая константа
            equal = 16,                 // =
            quote = 19,                 // '

            comma = 20,                 // ,
            star = 21,                  // *

            casesy = 31,                // case
            elsesy = 32,                // else
            gotosy = 33,                // goto
            typesy = 34,                //type
            withsy = 37,                //with

            assign = 51,	            //  :=
            thensy = 52,                // then
            untilsy = 53,               // until
            dosy = 54,                  //do
            ifsy = 56,                  //if
            filesy = 57,                //file

            slash = 60,                 // /
            point = 61,	                // .
            arrow = 62,	                // ^
            flpar = 63,	                // {
            frpar = 64,	                // }
            later = 65,	                // <
            greater = 66,	            // >
            laterequal = 67,	        //  <=
            greaterequal = 68,	        //  >=
            latergreater = 69,	        //  <>

            plus = 70,	                // +
            minus = 71,	                // –
            lcomment = 72,	            //  (*
            rcomment = 73,	            //  *)
            twopoints = 74,	            //  ..

            floatc = 82,                // вещественная константа
            writeln = 83,
            write = 84,
            read = 85,
            readln = 86,
            insy = 100,                 //in
            ofsy = 101,                 //of
            orsy = 102,                 //or
            tosy = 103,                 //to
            endsy = 104,                //end
            varsy = 105,                //var
            divsy = 106,                //div
            andsy = 107,                //and
            notsy = 108,                //not
            forsy = 109,                //for
            modsy = 110,                //mod
            nilsy = 111,                //nil
            setsy = 112,                //set
            beginsy = 113,              //begin
            whilesy = 114,              //while
            arraysy = 115,              //array
            constsy = 116,              //const
            labelsy = 117,              //label
            downtosy = 118,             //downto
            packedsy = 119,             //packed
            recordsy = 120,             //record
            repeatsy = 121,             //repeat
            programsy = 122,            //program
            functionsy = 123,           //function
            procedurensy = 124;         //proceduren
        static byte symbol; // код символа
        TextPosition token; // позиция символа
        public int nmb_int; // значение целой константы
        public static bool endworld = false;
        public string ident_name = "";
        static bool flag_pair = false; //флаг показывает, открыта ли скобка
        static bool flag_pair_1 = false;


        public byte NextSym()
        {
            while (InputOutput.Ch == ' ' || InputOutput.Ch == '\t')  //пропуск табуляций и пробелов
                InputOutput.NextCh();
            token.lineNumber = InputOutput.positionNow.lineNumber;  //Записываем номер строки
            token.charNumber = InputOutput.positionNow.charNumber;  //Записываем номер позиции в строке
            switch (InputOutput.Ch) //посимвольный выбор 
            {
                case char c when char.IsDigit(InputOutput.Ch): //целые числа 
                    if (InputOutput.Ch >= '0' && InputOutput.Ch <= '9') //промещуток 
                    {
                        byte digit;
                        Int16 maxint = Int16.MaxValue;  // максимальный предел целочисленной конастанты (32767)
                        nmb_int = 0;
                        while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')  //Пока цифры от 0 до 9
                        {
                            digit = (byte)(InputOutput.Ch - '0');
                            if (nmb_int < maxint / 10 || (nmb_int == maxint / 10 && digit <= maxint % 10))
                            {
                                nmb_int = 10 * nmb_int + digit;  //записываем каждый символ в свой разряд числа
                            }
                            else
                            {
                                // константа превышает предел
                                InputOutput.Error(203, InputOutput.positionNow);  //Добовляем ошибку о превышение предела
                                nmb_int = 0;  //Обнуляем число
                                while ((InputOutput.Ch >= '0' && InputOutput.Ch <= '9'))  //Пока цифры от 0 до 9
                                {
                                    InputOutput.NextCh();  //переходим на следующую позицию в строке
                                }
                            }
                            InputOutput.NextCh();  //переходим на следующую позицию в строке
                        }

                        symbol = intc;
                    }
                    break;
                case char c when char.IsLetter(InputOutput.Ch): //раздел юникода с буквами 
                    string name = ""; //строка для слова 
                    endworld = false;
                    while ((InputOutput.Ch >= 'a' && InputOutput.Ch <= 'z') || (InputOutput.Ch >= 'A' && InputOutput.Ch <= 'Z') || 
                        (InputOutput.Ch >= '0' && InputOutput.Ch <= '9') || (InputOutput.Ch >= 'А' && InputOutput.Ch <= 'Я') || 
                        (InputOutput.Ch >= 'а' && InputOutput.Ch <= 'я')) //пока все это находится 
                    {
                        name += InputOutput.Ch; //формируем слово 
                        if (InputOutput.positionNow.charNumber == InputOutput.lastInLine)  //Если номер позиции в строке равен концу строки 
                        {
                            InputOutput.NextCh();  //То переходим на следующую строку
                            break;  //выходим из switch
                        }
                        InputOutput.NextCh();  //переходим на следующую позицию в строке
                    }
                    endworld = true;
                    byte value;
                    //если нашлась соответствующая константа в словаре, то возвращаем её значение, иначе ident
                    if (Keywords.Kw.TryGetValue(name, out value))
                    {
                        symbol = value;  //Присваиваем значение ключевого слова
                    }
                    else
                    {
                        symbol = ident;  //Присваиваем знчения индетификатора
                    }
                    if (symbol == ident)
                        ident_name = name;
                    break;
                case '<':  //Кейс в котором записывается < или <= или <> 
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = laterequal;
                        InputOutput.NextCh();
                    }
                    else
                     if (InputOutput.Ch == '>')
                    {
                        symbol = latergreater;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = later;
                    }
                    break;
                case ':':  // Обработка оператора :=
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = assign;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = colon;
                        // Не считаем это ошибкой, так как : может быть частью :=
                    }
                    break;

                case ';':  //Кейс в котором записывается ;
                    symbol = semicolon;
                    InputOutput.NextCh();
                    break;
                case '/':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '/') //начало однострочного комментария
                    {
                        while (InputOutput.positionNow.charNumber != InputOutput.lastInLine)
                            InputOutput.NextCh(); //пропускаем всё до конца строки
                        InputOutput.NextCh();
                        NextSym();
                    }
                    else
                    {
                        InputOutput.Error(216, InputOutput.positionNow);
                        InputOutput.positionNow.charNumber = InputOutput.lastInLine;
                        InputOutput.NextCh();
                    }
                    NextSym();
                    break;
                case '.':  //Кейс в котором записывается . или ..
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '.')
                    {
                        symbol = twopoints;
                        InputOutput.NextCh();
                    }
                    else { symbol = point; }
                    break;
                case '(':  //Кейс в котором записывается (
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '*')  //начало многострочного комментария1
                    {
                        StartComment('*');
                        NextSym();
                    }
                    else
                    {
                        if (flag_pair_1)
                        {
                            InputOutput.Error(214, InputOutput.positionNow);
                        }
                        flag_pair_1 = true;  //открыть скобку
                        symbol = leftpar;
                    }
                    break;
                case ')':  //Кейс в котором записывается )
                    if (flag_pair_1)  //если до этого была открыта скобка
                        symbol = rightpar;
                    else
                        InputOutput.Error(215, InputOutput.positionNow);  //ошибка на не открытый комментарий
                    flag_pair_1 = false;  //закрыть скобку
                    InputOutput.NextCh();
                    break;
                case '{':  //Кейс в котором записывается {
                    symbol = flpar;
                    flag_pair = true;
                    StartComment('{');
                    NextSym();
                    break;
                case '}':  //Кейс в котором записывается }
                    if (flag_pair)
                    {
                        symbol = frpar;
                    }
                    else
                    {
                        InputOutput.Error(211, InputOutput.positionNow);
                        InputOutput.NextCh();
                        symbol = 0;
                        flag_pair = false;
                    }
                    InputOutput.NextCh();
                    break;
                default:  //обычная константа, не требующая особой обработки
                    name = InputOutput.Ch.ToString();
                    //если она есть в талице ключевых значений
                    if ((Keywords.Kw.TryGetValue(name, out value)))
                    {
                        symbol = value;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        InputOutput.Error(6, InputOutput.positionNow);
                        InputOutput.NextCh();
                        symbol = 0;
                    }
                    break;
            }

            //запись кода в файл
            StreamWriter Simantic = new StreamWriter("Simantic.txt", true);
            if (symbol != 0 && InputOutput.flagOpenFile)  //если с символом не возникло ошибки и файл открыт
                Simantic.Write(symbol + " ");  //заносим коды символов в файл
            if (InputOutput.positionNow.charNumber == 0 && InputOutput.flagOpenFile)  // в конце строки перенос на новую строку
                Simantic.Write('\n');  //переход на новую строку 
            Simantic.Close();
            return symbol;  
        }

        static public void StartComment(char a)
        {
            TextPosition start_p = InputOutput.positionNow;
            if (a == '{')  //если многострочный комментарий
            {
                while (InputOutput.Ch != '}')  //пока не встретился символ закрытия
                {
                    InputOutput.NextCh();  //читаем следующий символ
                    if (InputOutput.positionNow.charNumber == InputOutput.lastInLine)
                    {
                        InputOutput.Error(212, start_p);
                        symbol = 0;
                        return;
                    }
                }
                InputOutput.NextCh();
                symbol = 0;
                return;

            }
            if (a == '*')  //если многострочный комментарий 
            {
                while (InputOutput.positionNow.charNumber != InputOutput.lastInLine)
                {
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '*')
                    {
                        InputOutput.NextCh();
                        if (InputOutput.Ch == ')')
                        {
                            InputOutput.NextCh();
                            symbol = 0;
                            return;
                        }
                    }

                }
                if (InputOutput.positionNow.charNumber == InputOutput.lastInLine)
                {
                    InputOutput.Error(213, start_p);
                    symbol = 0;
                    return;
                }
            }
            return;
        }
    }
}
