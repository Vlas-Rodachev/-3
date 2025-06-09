namespace Компилятор
{
    class Program
    {
        static void Main()
        {
            string file = "input5.txt";

            StreamWriter writer = new StreamWriter("Simantic.txt");  //Открытие файла для записи для того чтобы его очистить
            writer.Close();

            InputOutput.File = new StreamReader(file);  //Открытие основного файла для чтения програмного кода PASCAL 
            while (!(InputOutput.File.EndOfStream))  //Пока не конец файла
            {
                SyntaxAnalis.SyntaxAlanyzer();
            }
            SyntaxAnalis.SyntaxAlanyzer();
            InputOutput.End();  //Завершение программы
        }
    }
}
