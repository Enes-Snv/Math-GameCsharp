using System; // Temel .NET sınıflarını kullanmak için.
using System.Collections.Generic; // List gibi koleksiyon sınıfları için.
using System.ComponentModel; // Bileşen modelleme sınıfları için.
using System.Data; // Veri ile ilgili işlemler için.
using System.Drawing; // Grafik ve renk işlemleri için.
using System.Linq; // LINQ sorguları için.
using System.Text; // Metin işlemleri için.
using System.Threading.Tasks; // Asenkron görevler için.
using System.Windows.Forms; // Windows Forms bileşenlerini kullanmak için.
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar; // Visual Style öğeleri için.

namespace Matematik_Oyunu // Proje için ad alanı.
{
    public partial class Form2 : Form // Form2 adında bir form tanımlanıyor.
    {
        int firstNumber; // Matematik sorusu için birinci sayı.
        int secondNumber; // Matematik sorusu için ikinci sayı.
        int correctAnswer; // Doğru cevabı tutacak değişken.
        Random rand = new Random(); // Rastgele sayılar üretmek için.

        public enum Player // Oyuncular için bir enum (X ve O).
        {
            X, O
        }

        Player currentPlayer; // Şu anda oynayan oyuncuyu tutar.
        Random random = new Random(); // Bilgisayarın hamleleri için rastgele seçim.
        int playerWinCount = 0; // Oyuncunun kazandığı oyun sayısı.
        int CPUWinCount = 0; // Bilgisayarın kazandığı oyun sayısı.
        List<Button> buttons; // Oyunda kullanılan butonları tutacak liste.

        public Form2() // Form2'nin yapıcı metodu.
        {
            InitializeComponent(); // Form bileşenlerini başlatır.
            CreateMathFunc(); // İlk matematik işlemini oluşturur.
            RestartGame(); // Oyunu başlatır.
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde yapılacak işlemler. Şu an boş.
        }

        private void CPUmove(object sender, EventArgs e)
        {
            if (buttons.Count > 0) // Eğer oynanabilir butonlar varsa.
            {
                int index = random.Next(buttons.Count); // Rastgele bir buton seç.
                buttons[index].Enabled = false; // Butonu devre dışı bırak.
                currentPlayer = Player.O; // Bilgisayarın hamlesi O.
                buttons[index].Text = currentPlayer.ToString(); // Butona "O" yaz.
                buttons[index].BackColor = Color.DarkSalmon; // Buton rengini değiştir.
                buttons.RemoveAt(index); // Butonu oynanabilir listeden çıkar.
                CheckGame(); // Oyun bitiş durumunu kontrol et.
                CPUTimer.Stop(); // Bilgisayar hareket zamanlayıcısını durdur.
            }
        }

        private void PlayerClickButton(object sender, EventArgs e)
        {
            if (!CheckMathAnswer()) // Eğer cevap yanlışsa.
            {
                MessageBox.Show("Yanlış cevap! Lütfen doğru cevabı girin."); // Hata mesajı göster.
                return; // İşlemi durdur.
            }

            var button = (Button)sender; // Tıklanan butonu al.

            currentPlayer = Player.X; // Oyuncunun hamlesi X.
            button.Text = currentPlayer.ToString(); // Butona "X" yaz.
            button.Enabled = false; // Butonu devre dışı bırak.
            button.BackColor = Color.Cyan; // Buton rengini değiştir.
            buttons.Remove(button); // Butonu oynanabilir listeden çıkar.
            CheckGame(); // Oyun bitiş durumunu kontrol et.
            CPUTimer.Start(); // Bilgisayar hareket zamanlayıcısını başlat.
            CreateMathFunc(); // Yeni bir matematik sorusu oluştur.
        }

        private void RestartGame(object sender, EventArgs e)
        {
            RestartGame(); // Oyunu yeniden başlat.
        }

        private void CheckGame()
        {
            // Kazanan durumlarını kontrol eder. Oyuncu X veya Bilgisayar O kazanabilir.
            if (button1.Text == "X" && button2.Text == "X" && button3.Text == "X" // Üst satır kontrolü.
                || button4.Text == "X" && button5.Text == "X" && button6.Text == "X" // Orta satır kontrolü.
                || button7.Text == "X" && button8.Text == "X" && button9.Text == "X" // Alt satır kontrolü.
                || button1.Text == "X" && button4.Text == "X" && button7.Text == "X" // Sol sütun kontrolü.
                || button2.Text == "X" && button5.Text == "X" && button8.Text == "X" // Orta sütun kontrolü.
                || button3.Text == "X" && button6.Text == "X" && button9.Text == "X" // Sağ sütun kontrolü.
                || button1.Text == "X" && button5.Text == "X" && button9.Text == "X" // Çapraz kontrol (sol üstten sağ alta).
                || button3.Text == "X" && button5.Text == "X" && button7.Text == "X" // Çapraz kontrol (sağ üstten sol alta).
                )
            {
                CPUTimer.Stop(); // Bilgisayar hareket zamanlayıcısını durdur.
                MessageBox.Show("Oyuncu Kazandı"); // Kazanma mesajı göster.
                playerWinCount++; // Oyuncu puanını artır.
                label1.Text = "Oyuncu: " + playerWinCount; // Oyuncu skorunu güncelle.
                RestartGame(); // Oyunu yeniden başlat.
            }
            else if (button1.Text == "O" && button2.Text == "O" && button3.Text == "O"
                || button4.Text == "O" && button5.Text == "O" && button6.Text == "O" 
                || button7.Text == "O" && button8.Text == "O" && button9.Text == "O" 
                || button1.Text == "O" && button4.Text == "O" && button7.Text == "O" 
                || button2.Text == "O" && button5.Text == "O" && button8.Text == "O" 
                || button3.Text == "O" && button6.Text == "O" && button9.Text == "O" 
                || button1.Text == "O" && button5.Text == "O" && button9.Text == "O" 
                || button3.Text == "O" && button5.Text == "O" && button7.Text == "O"// Aynı kontroller Bilgisayar için.
                                                                                  // Benzer şekilde diğer kazanan durumlar kontrol ediliyor...
                     )
            {
                CPUTimer.Stop();
                MessageBox.Show("Bilgisayar Kazandı");
                CPUWinCount++;
                label2.Text = "Bilgisayar: " + CPUWinCount;
                RestartGame();
            }
        }

        private void RestartGame()
        {
            // Tüm butonları başlangıç durumuna döndür.
            buttons = new List<Button> { button1, button2, button3, button4, button5, button6, button7, button8, button9 };

            foreach (Button x in buttons)
            {
                x.Enabled = true; // Butonları aktif yap.
                x.Text = "?"; // Butonlara başlangıç değerini koy.
                x.BackColor = DefaultBackColor; // Buton renklerini sıfırla.
            }
            CreateMathFunc(); // Yeni bir matematik sorusu oluştur.
        }

        private void CreateMathFunc()
        {
            int operationType = rand.Next(0, 2); // Rastgele işlem türü seç (0: çarpma, 1: bölme).

            if (operationType == 0)
            {
                firstNumber = rand.Next(1, 10);
                secondNumber = rand.Next(1, 10);
                correctAnswer = firstNumber * secondNumber;
                lblCurrentOperation.Text = $"{firstNumber} x {secondNumber}"; // Soru metnini göster.
            }
            else
            {
                do
                {
                    firstNumber = rand.Next(1, 31);
                    secondNumber = rand.Next(1, 10);
                } while (firstNumber % secondNumber != 0); // Tam bölünebilirlik kontrolü.

                correctAnswer = firstNumber / secondNumber;
                lblCurrentOperation.Text = $"{firstNumber} ÷ {secondNumber}";
            }
        }

        private bool CheckMathAnswer()
        {
            int userAnswer;
            if (int.TryParse(txtAnswer.Text, out userAnswer)) // Kullanıcı cevabını sayıya çevir.
            {
                return userAnswer == correctAnswer; // Doğru cevabı kontrol et.
            }

            MessageBox.Show("Lütfen geçerli bir sayı girin!"); // Hatalı giriş mesajı.
            return false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Close(); // Bu formu kapat.
            Form form1 = new Form1(); // Form1'i başlat.
            form1.Show(); // Form1'i görüntüle.
        }
    }
}
