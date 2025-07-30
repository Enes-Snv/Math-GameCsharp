using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Matematik_Oyunu // Projenin ad alanı.
{
    public partial class Form3 : Form // Form3 sınıfı, Windows Form sınıfını genişletiyor.
    {
        int speed; // Balonların hareket hızı.
        int score; // Oyuncunun puanı.
        Random rand = new Random(); // Rastgele sayı üretmek için.
        bool gameOver; // Oyunun bitip bitmediğini kontrol eden değişken.

        int firstNumber; // Matematik işlemi için birinci sayı.
        int secondNumber; // Matematik işlemi için ikinci sayı.
        int correctAnswer; // Doğru cevabı tutacak değişken.
        PictureBox answerBalloon; // Doğru cevabı içeren balonu tutacak değişken.

        public Form3()
        {
            InitializeComponent(); // Form bileşenlerini başlatır.
            RestartGame(); // Oyunu başlatır.
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Form3 yüklendiğinde yapılacak işlemler. Şu an boş bırakılmış.
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = "Score: " + score; // Ekrana puanı yazdırır.

            if (gameOver) // Eğer oyun bittiyse.
            {
                gameTimer.Stop(); // Zamanlayıcıyı durdur.
                txtScore.Text = "Score: " + score + " Oyun bitti."; // Oyun bitiş mesajı.
                return; // Daha fazla işlem yapılmaz.
            }

            // Formdaki her kontrolü kontrol et.
            foreach (Control x in this.Controls)
            {
                // txtScore, button1 ve lblCurrentOperation dışında kalan kontroller.
                if (x != txtScore && x != button1 && x != lblCurrentOperation)
                {
                    x.Top -= speed; // Balonları yukarı doğru hareket ettir.

                    if (x.Top < -100 && (string)x.Tag == "balloon") // Eğer balon yukarıdan çıkarsa.
                    {
                        gameOver = true; // Oyun biter.
                        txtScore.Text = "Score: " + score + " Oyun bitti, yeniden başlamak için Enter'a basın.";
                        return;
                    }

                    if ((string)x.Tag == "balloon") // Eğer balon bir bombaya çarparsa.
                    {
                        if (bomb.Bounds.IntersectsWith(x.Bounds))
                        {
                            SetRandomPosition(x); // Balon yeniden yerleştirilir.
                        }
                    }
                }
            }
        }

        private void PopBalloon(object sender, EventArgs e)
        {
            if (!gameOver) // Eğer oyun devam ediyorsa.
            {
                var balloon = (PictureBox)sender; // Tıklanan balonu al.

                if (balloon == answerBalloon && balloon.Text == correctAnswer.ToString())
                {
                    score += 5; // Doğru cevap için ekstra puan ekle.
                }
                else
                {
                    score -= 2; // Yanlış cevap için puanı azalt.
                }

                SetRandomPosition(balloon); // Balonu yeniden yerleştir.
                GenerateRandomOperation(); // Yeni bir matematik işlemi oluştur.
            }
        }

        private void GoBoom(object sender, EventArgs e)
        {
            if (!gameOver) // Eğer oyun devam ediyorsa.
            {
                bomb.Image = Properties.Resources.boom; // Bombanın görselini değiştir.
                gameOver = true; // Oyun biter.
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && gameOver) // Eğer oyun bitti ve Enter tuşuna basıldıysa.
            {
                RestartGame(); // Oyunu yeniden başlat.
            }
        }

        private void RestartGame()
        {
            speed = 5; // Başlangıç hızı.
            score = 0; // Başlangıç puanı.
            gameOver = false; // Oyunun durumu.

            bomb.Image = Properties.Resources.bomb; // Bombanın varsayılan görseli.

            foreach (Control x in this.Controls)
            {
                if ((string)x.Tag == "balloon" && x is PictureBox balloon)
                {
                    SetRandomPosition(x); // Balonu rastgele bir pozisyona yerleştir.
                    balloon.Text = ""; // Balon metinlerini temizle.
                    balloon.Paint -= Balloon_Paint; // Paint olayını temizle.
                    balloon.Paint += Balloon_Paint; // Paint olayını bağla.
                }
            }

            txtScore.BringToFront(); // Puan etiketini öne getir.
            lblCurrentOperation.BringToFront(); // İşlem etiketini öne getir.
            GenerateRandomOperation(); // Yeni bir matematik işlemi oluştur.
            gameTimer.Start(); // Zamanlayıcıyı başlat.
        }

        private void GenerateRandomOperation()
        {
            // Rastgele bir matematik işlemi oluştur (toplama veya çıkarma).
            int operationType = rand.Next(0, 2);

            if (operationType == 0) // Çıkarma işlemi.
            {
                firstNumber = rand.Next(0, 21);
                secondNumber = rand.Next(0, 10);
                correctAnswer = firstNumber - secondNumber;
                lblCurrentOperation.Text = $"{firstNumber} - {secondNumber} = ?";
            }
            else // Toplama işlemi.
            {
                firstNumber = rand.Next(0, 21);
                secondNumber = rand.Next(0, 21);
                correctAnswer = firstNumber + secondNumber;
                lblCurrentOperation.Text = $"{firstNumber} + {secondNumber} = ?";
            }

            // Doğru cevabı bir balona yaz.
            List<PictureBox> balloons = this.Controls.OfType<PictureBox>()
                .Where(b => (string)b.Tag == "balloon").ToList();

            if (balloons.Count > 0)
            {
                answerBalloon = balloons[rand.Next(balloons.Count)];
                answerBalloon.Text = correctAnswer.ToString();
                answerBalloon.Invalidate(); // Balonu yeniden çiz.
            }
        }

        private void SetRandomPosition(Control balloon)
        {
            // Balon için rastgele bir pozisyon belirle.
            int x, y;
            do
            {
                x = rand.Next(5, this.ClientSize.Width - balloon.Width);
                y = rand.Next(700, 1000);
            } while (IsOverlapping(balloon, x, y)); // Çakışma kontrolü.

            balloon.Left = x; // X konumu.
            balloon.Top = y; // Y konumu.
        }

        private bool IsOverlapping(Control balloon, int x, int y)
        {
            foreach (Control other in this.Controls)
            {
                if (other != balloon && (string)other.Tag == "balloon")
                {
                    Rectangle newBounds = new Rectangle(x, y, balloon.Width, balloon.Height);
                    if (newBounds.IntersectsWith(other.Bounds)) // Çakışma kontrolü.
                    {
                        return true; // Çakışma varsa.
                    }
                }
            }
            return false; // Çakışma yoksa.
        }

        private void Balloon_Paint(object sender, PaintEventArgs e)
        {
            // Balonun üzerinde metin yazdırma.
            PictureBox balloon = (PictureBox)sender;

            if (!string.IsNullOrEmpty(balloon.Text))
            {
                using (Font font = new Font("Arial", 14, FontStyle.Bold))
                {
                    SizeF textSize = e.Graphics.MeasureString(balloon.Text, font);
                    e.Graphics.DrawString(balloon.Text, font, Brushes.Black,
                        (balloon.Width - textSize.Width) / 2,
                        (balloon.Height - textSize.Height) / 2);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Form3'ü kapatıp Form1'i başlatır.
            this.Close();
            Form form1 = new Form1();
            form1.Show();
        }
    }
}
