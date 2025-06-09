using System;
using System.Collections.Generic;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Компилятор
{
    struct TextPosition//Структура Позиция в файле
    {
        public uint lineNumber; // номер строки
        public byte charNumber; // номер позиции в строке

        public TextPosition(uint ln = 0, byte c = 0) //Конструктор
        {
            lineNumber = ln; //линия 
            charNumber = c; //столбик 
        }
    }
    struct Err // стуктура ошибок 
    {
        public TextPosition errorPosition;// позиция ошибки
        public byte errorCode; // код ошибки

        public Err(TextPosition errorPosition, byte errorCode) //конструктор 
        {
            this.errorPosition = errorPosition;
            this.errorCode = errorCode;
        }
    }
    class InputOutput
    {
        const byte ERRMAX = 9; // макс. число ошибок в строке, которые увидит пользователь
        public static char Ch = ' '; // текущий символ
        public static TextPosition positionNow = new TextPosition(); // указатель на текущую позицию
        static string line = ""; // считанная строка
        public static byte lastInLine = 1;// последний символ в строке
        public static List<Err> err;// список ошибок
        public static StreamReader File { get; set; }// файл для чтения
        public static uint errCount = 0;// количество ошибок
        public static bool flagOpenFile = true;// флаг чтения
        public static bool startstring, temp = false;
        /*public static bool fornext = false;*/

        static public void NextCh()  // метод который переходит к следующему символу 
        {
            if (positionNow.charNumber == lastInLine)  //Если номер позиции в строке равен концу строки 
            {

                if (errCount > 0)  //Если кол-во ошибок не равно нулю, то выводим их
                {
                    ListErrors();
                }

                if (flagOpenFile)  //Если flagOpenFile = true продолжаем считывать из файла
                    ReadNextLine();  //переход к методу 
                positionNow.lineNumber = positionNow.lineNumber + 1;  //Добовляем номер строки
                positionNow.charNumber = 0;  //Сбрасываем позицию в строке
                startstring = true;
                temp = false;
            }
            else
            {
                if (Ch != ' ' && temp && LexicalAnalyzer.endworld)
                {
                    startstring = false;
                }
                if (Ch != ' ' && !temp)
                {
                    temp = true;
                }
                ++positionNow.charNumber; // если не конец строки то значит еще есть символы, передвигаемся на +1 
            }
            if (line.Length != 0)  //Если длина строки не равна 0,то в Ch записываем позицию в строке
                Ch = line[positionNow.charNumber];  // указываем символ который находится по строке и столбцу 
        }

        private static void ReadNextLine()  // метод чтения следующей строки 
        {
            if (!(File.EndOfStream))  // если не конец файла
            {
                line = File.ReadLine();  //Считываем строку файла
                Console.WriteLine(line);  //Выводим строку

                err = new List<Err>();  // создание листа с ошибками 
                if (line.Length != 0)  //Если длина строки не равна 0
                {
                    lastInLine = Convert.ToByte(line.Length - 1);  //Высчитваем позицию конца строки
                }
                else
                {
                    lastInLine = 0;
                }
            }
            else
            {
                End();  //завершение программы
            }
        }
        public static void End() //Завершение программы
        {
            flagOpenFile = false; //окончание чтения
            if (!SyntaxAnalis.endp && SyntaxAnalis.begino)
            {
                InputOutput.Error(61, InputOutput.positionNow);
                Console.WriteLine("**" + errCount + "**" + "^ ошибка код 61");
                Console.WriteLine("Ожидалась точка");
            }

            Console.ForegroundColor = (ConsoleColor)10;  //покарска сообщения о компиляции 
            if (errCount == 0)  //Если ошибок нет 
                Console.ForegroundColor = (ConsoleColor)10;  //Цвет текста в консоли зеленый
            else
                Console.ForegroundColor = (ConsoleColor)12;  //Иначе красный

            Console.WriteLine($"Компиляция завершена: : ошибок — {errCount}!"); //Вывод кол-ва ошибок 
            Console.ReadLine();
            Environment.Exit(0); //завершение программы 
        }
        static public void ListErrors()
        {
            int pos = 1 - $"{positionNow.lineNumber} ".Length;  //вычисление позции ошибки
            string s;
            foreach (Err item in err)
            {
                s = "**";  // добавить в строку **
                if (errCount < 10) s += "0";  // добавить в строку 0 перед числами 1..9
                s += $"{errCount}**";  // добавить в строку кол-во ошибок
                while (s.Length - 1 < pos + item.errorPosition.charNumber) s += " ";  // добавить пробелы до позиции символа с ошибкой
                s += $"^ ошибка код {item.errorCode}";  // добавить в строку указатель и код ошибки
                Console.WriteLine(s);
                string value;
                if (Erorr.codeerror.TryGetValue(item.errorCode, out value))
                {
                    Console.WriteLine("****** " + Erorr.codeerror[item.errorCode]);
                    Console.WriteLine();
                }
            }

        }
        static public void Error(byte errorCode, TextPosition position)
        {
            errCount++; // увеличить кол-во ошибок
            Err e;
            if (errCount <= ERRMAX) // если кол-во ошибок не больше максимального
            {
                e = new Err(position, errorCode);  // создать новую ошибку
                err.Add(e);  // добавить её в список
            }
        }
    }
}