using System;
using NAudio.Wave;
using Newtonsoft.Json;
using Vosk;
using System.IO.Ports;

namespace VoskRecognitionExample
{
    public class Program
    {
        private static WaveInEvent audioWaveMicrophone;
        private static VoskRecognizer voiceRecognizer ;
        private static SerialPort sp;
        private static bool state = false;

        static void Main(string[] args)
        {
            //foreach (var port in SerialPort.GetPortNames())  // Выводит в консоль все существующие COM-порты
            //{
            //    Console.WriteLine(port);
            //}
            sp = new SerialPort("COM3", 9600); // заменить на правильный порт
            sp.Open();

            Vosk.Vosk.SetLogLevel(-1);
            Vosk.Vosk.GpuThreadInit();

            voiceRecognizer = new VoskRecognizer(new Model("vosk-model-small-ru-0.22"), 16000);

            audioWaveMicrophone = new WaveInEvent();
            audioWaveMicrophone.WaveFormat = new WaveFormat(16000, 1); // тут не трогаем
            audioWaveMicrophone.DataAvailable += AudioWaveMicrophone_DataAvailable;
            audioWaveMicrophone.BufferMilliseconds = 20; // Меньше - быстрее реакция, но меньше точность. Больше - медленнее, но точнее
            audioWaveMicrophone.StartRecording();

            Console.ReadLine();
        }

        private static void AudioWaveMicrophone_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (voiceRecognizer.AcceptWaveform(e.Buffer, e.Buffer.Length))
            {
                string result = voiceRecognizer.Result();
                RecognitionAction(result);
            }
            else
            {
                string result = voiceRecognizer.PartialResult();
                RecognitionAction(result);
            }
        }

        private static void RecognitionAction(string text)
        {
            Result result = JsonConvert.DeserializeObject<Result>(text);
            if (!string.IsNullOrEmpty(result.partial)) // Часть сказанного
            {
                Console.WriteLine(result.partial);
                switch(result.partial)
                {
                    case string s when s.Contains("включи") && state == false:
                        sp.Write(new byte[1] { 1 }, 0, 1);
                        state = true;
                        break;
                    case string s when s.Contains("выключи") && state:
                        sp.Write(new byte[1] { 0 }, 0, 1);
                        state = false;
                        break;
                }
            }
            if (!string.IsNullOrEmpty(result.text)) // Полностью сказанное
            {
                Console.Clear();
                Console.WriteLine(result.text);
                if (result.text.Contains("сигнал"))
                {
                    Console.Beep();
                }
            }
        }
    }
    public class Result // название переменных менять нельзя
    {
        public string partial;
        public string text;
    }
}