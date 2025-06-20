﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компилятор
{
    class Erorr
    {
        public static Dictionary<byte, string> codeerror = new Dictionary<byte, string>() //словарь ошибок
        {

            [2] = "Неверный идентификатор",
            [5] = "Неверное :",
            [6] = "Неверный символ",
            [11] = "Ожидаллось [",
            [12] = "Ожидаллось ]",
            [14] = "Ожидалось ;",
            [15] = "Неверный var",
            [51] = "Ожидалось :=",
            [61] = "Ожидалась точка",
            [74] = "Ожидалось ..",
            [94] = "Данный тип не существует",
            [88] = "Данная переменна не объявлялась в var",
            [101] = "Ожидался of",
            [113] = "Ожидался begin",
            [200] = "Повторное объявление переменной",
            [201] = "Несовместимые типы в операции присваивания",
            [202] = "Попытка индексации не-массива",
            [203] = "Выход за границы массива",
            [204] = "Недопустимая операция для данного типа",
            [211] = "Ожидалось {",
            [212] = "Ожидалось }",
            [213] = "Ожидалась конструкция (* *)",
            [214] = "Ожидалась закрытая скобка",
            [215] = "Лишняя скобка ",
            [216] = "Ожидалось //"
        };
    }
}
