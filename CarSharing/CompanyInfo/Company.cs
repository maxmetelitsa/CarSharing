using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using CarSharing.CarsInfo;

namespace CarSharing.CompanyInfo
{
    public class Company
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string CorrespondentAccount { get; set; }
        public string BIK { get; set; }
        public string INN { get; set; }

        public Company(string name, string address, string phone, string email, string bankName, string accountNumber, string correspondentAccount, string bik, string inn)
        {
            Name = name;
            Address = address;
            Phone = phone;
            Email = email;
            BankName = bankName;
            AccountNumber = accountNumber;
            CorrespondentAccount = correspondentAccount;
            BIK = bik;
            INN = inn;
        }

        public void GenerateRentalDocument(string outputPath, Car car, string name, string surname, string email, string passportNumber, string identificationNumber, string licenseSeries, string licenseNumber, DateTime startDate, DateTime endDate, int days, double total)
        {
            // Создание документа Word
            using (WordprocessingDocument document = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
            {
                // Добавление основного раздела документа
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();

                // Создание содержимого документа
                Body body = mainPart.Document.AppendChild(new Body());

                Paragraph titleParagraph = CreateParagraph("\t\t\t\tДОГОВОР АРЕНДЫ АВТОМОБИЛЯ", "\n");
                body.AppendChild(titleParagraph);

                body.AppendChild(CreateParagraph("Car House - аренда автомобилей ", Address));
                body.AppendChild(CreateParagraph("Телефон: ", Phone));
                body.AppendChild(CreateParagraph("Электронная почта: ", Email));
                body.AppendChild(CreateParagraph("Вебсайт: ", "www.carhouse.by"));

                body.AppendChild(CreateParagraph(" ", "\n")); 
                // Добавление информации об Арендаторе и Арендодателе
                string contractText = $"{Name}, действующий на основании свидетельства 12 No BYK765327828  от 01.01.2023, выданного Министерством юстиции Республики Беларусь, именуемый в дальнейшем «Арендодатель» и {name} {surname}, именуемый в дальнейшем «Арендатор», совместно именуемые «Стороны» заключили настоящий Договор о нижеследующем:";
                body.AppendChild(CreateParagraph(contractText, "\n"));

                // Добавление информации о предмете договора
                string subjectText = "\n1. ПРЕДМЕТ ДОГОВОРА";
                body.AppendChild(CreateParagraph(subjectText, "\n"));
                body.AppendChild(CreateSingleParagraph($"1.1. Арендодатель предоставляет Арендатору автомобиль марки «{car.Brand}», {car.YearOfManufacture} года выпуска, государственный номер {car.Number}, принадлежащий Арендодателю на праве собственности; за плату во временное владение и пользование без оказания услуг по управлению им и его технической эксплуатации."));
                body.AppendChild(CreateSingleParagraph($"1.2. Стоимость подневной аренды автомобиля составляет: {car.DayPrice} долларов США."));
                body.AppendChild(CreateSingleParagraph("1.3. Комплектность автомобиля, передаваемого в аренду, а также перечень передаваемых документов на автомобиль определен Актом приема-передачи (Приложение 1), являющимся неотъемлемой частью настоящего Договора."));
                body.AppendChild(CreateSingleParagraph("1.4. Использование автомобиля не должно противоречить его назначению, техническим характеристикам и положениям настоящего Договора."));
                body.AppendChild(CreateSingleParagraph("1.5. Арендовать автомобиль и управлять им может только Арендатор, у которого возраст не менее 20 лет, водительский стаж не менее 2 лет."));

                // Добавление прав и обязанностей сторон
                string rightsAndDutiesText = "\n2. ПРАВА И ОБЯЗАННОСТИ СТОРОН";
                body.AppendChild(CreateParagraph(rightsAndDutiesText, "\n"));
                body.AppendChild(CreateSingleParagraph("2.1. Арендодатель предоставляет автомобиль в исправном состоянии и в чистом виде."));
                body.AppendChild(CreateSingleParagraph("2.2. Арендодатель обязуется за свой счет производить все виды необходимого ремонта автомобиля (в том числе текущие и капитальные), своевременное профилактическое обслуживание автомобиля. Арендодатель обязуется на период профилактического обслуживания заменить его на иной автомобиль в надлежащем состоянии, имеющийся в наличии Арендодателя. При отсутствии возможности для такой замены действие Договора считается досрочно прекращенным, предмет договора возвращается Арендодателю, а оплата за арендуемый автомобиль взимается за то время, в течение которого он фактически находился в распоряжении Арендатора на основании настоящего Договора."));
                body.AppendChild(CreateSingleParagraph("2.3. Арендодатель несет расходы по страхованию автомобиля, в порядке, установленном настоящим Договором."));
                body.AppendChild(CreateSingleParagraph("2.4. Арендатор своими силами осуществляет управление арендованным автомобилем и его эксплуатацию."));
                body.AppendChild(CreateSingleParagraph("2.5. Арендатор перед началом эксплуатации автомобиля обязан ознакомиться с правилами пользования (см. руководство)."));
                body.AppendChild(CreateSingleParagraph("2.6. Арендатор не имеет право производить разборку и ремонт автомобиля, производить вмешательство в конструкцию автомобиля и устанавливать на него дополнительное оборудование, использовать автомобиль для буксировки любых транспортных средств, для езды с прицепом или по бездорожью (дорогам не имеющим твердого покрытия), принимать участие в спортивных соревнованиях, обучение вождению, осуществления коммерческой перевозки пассажиров и грузов без разрешения Арендодателя, эксплуатировать автомобиль в случае аварии или поломки, сдавать автомобиль в субаренду, заключать с третьими лицами договоры перевозки, в ходе которых используется автомобиль."));
                body.AppendChild(CreateSingleParagraph("2.7. Арендатор несет все расходы, связанные с эксплуатацией автомобиля, а также за свой счет оплачивает парковку и все штрафы за нарушение Правил Дорожного Движения. Арендатор не имеет право передавать управление автомобилем другим лицам, если они не вписаны в договор."));
                body.AppendChild(CreateSingleParagraph("2.9.Арендатор обязуется по истечении срока действия договора возвратить Арендодателю предоставленный автомобиль в полной комплектации, по акту приёма - передачи(возврат после аренды, приложение 3), в надлежащем техническом состоянии(без ухудшения его потребительских качеств и внешнего вида). Арендатор должен вернуть Автомобиль в чистом виде или оплатить стоимость мойки и уборки салона. Арендатор должен заправить автомобиль перед возвращением его Арендодателю и предоставить чек с АЗС или заплатить за заправку из расчета стоимости топлива на АЗС, если количество топлива при возврате автомобиля меньше, чем при получении."));
                body.AppendChild(CreateSingleParagraph("2.10.Арендатор обязуется: "));
                body.AppendChild(CreateSingleParagraph("2.10.1.Постоянно хранить при себе документы на автомобиль(не оставлять в автомобиле). За утерю ключей или документов штраф 200 USD."));
                body.AppendChild(CreateSingleParagraph("2.10.2.Производить регулярную тщательную проверку автомобиля на наличие внешних и внутренних неисправностей, уровня масла ДВС, охлаждающей жидкости, тормозной жидкости, и при их несоответствии незамедлительно ставить в известность Арендодателя. В случае загорания контрольных ламп(check engine и т.п.), сигнализирующих о неисправности систем автомобиля, немедленно сообщить Арендодателю, не предпринимать мер по самостоятельному устранению неисправностей."));
                body.AppendChild(CreateSingleParagraph("2.10.3.По первому требованию Арендодателя предоставлять автомобиль для прохождения технического осмотра."));
                body.AppendChild(CreateSingleParagraph("2.10.4.Предоставлять автомобиль для проведения технического обслуживания через каждые 20 тыс км. пробега после получения его в аренду. Обеспечить возможность Арендодателю по согласованию с Арендатором осмотреть автомобиль на предмет его технического состояния и правильной эксплуатации."));

                string responsibilitiesOfTheParties = "\n3. ОТВЕТСТВЕННОСТЬ СТОРОН ";
                body.AppendChild(CreateParagraph(responsibilitiesOfTheParties, "\n"));
                body.AppendChild(CreateSingleParagraph("3.2.1.За задержку при возврате автомобиля, если это не согласовано с Арендодателем согласно п.4.4."));
                body.AppendChild(CreateSingleParagraph("3.2.2.Не выполнен какой-либо из подпунктов пунктов 2.10, 2.11, 2.12, или 2.13 настоящего договора. При наличии вины Арендатора или отсутствия справки из ГАИ."));
                body.AppendChild(CreateSingleParagraph("3.3.Арендатор несет ответственность за сохранность арендуемого автомобиля в течение всего срока аренды до момента передачи его Арендодателю. В случае, если при возвращении автомобиля он имеет неисправности либо комплектацию, отличную от указанной в Акте приема - передачи, и отсутствуют документы, подтверждающие факт ДТП или противоправные действия третьих лиц из ГАИ или милиции, Арендатор уплачивает Арендодателю стоимость восстановительного ремонта поврежденного автомобиля в той части, которая не покрывается страховым возмещением от страховой компании, а также оплачивает Арендодателю упущенную выгоду, вызванную простоем автомобиля, за время ликвидации ущерба."));
                body.AppendChild(CreateSingleParagraph("3.4.Уплата штрафа не освобождает Арендатора от выполнения обязательств по оплате основного долга за аренду."));

                string settlementProcedure = "\n4. ПОРЯДОК РАСЧЕТОВ ";
                body.AppendChild(CreateParagraph(settlementProcedure, "\n"));
                body.AppendChild(CreateSingleParagraph("4.1.Размер и порядок выплаты Арендатором Арендодателю арендной платы по настоящему Договору установлен в Калькуляции стоимости аренды автомобиля(Приложение 2), являющееся неотъемлемой частью настоящего договора."));
                body.AppendChild(CreateSingleParagraph("4.2.При передаче автомобиля Арендатору, последний осуществляет 100 % предоплату за аренду автомобиля, за согласованные сроки эксплуатации, в размере определенном в Приложении No2 к настоящему Договору. Вносит залог, который возвращается при возврате автомобиля в исправном состоянии, без повреждений, в полной комплектации по акту приема - передачи и без просроченного срока аренды."));

                string insurance = "\n5. СТРАХОВАНИЕ ";
                body.AppendChild(CreateParagraph(insurance, "\n"));
                body.AppendChild(CreateSingleParagraph("5.1.Автомобиль застрахован на условиях гражданской ответственности(до 10000 Евро) и полного АВТОКАСКО.Страховые взносы включены в тариф.Договором страхования АВТОКАСКО покрываются риски, связанные с утратой и повреждением транспортного средства в результате аварии, самовозгорания, пожара, взрыва, природных сил и стихийных бедствий, падения на застрахованное транспортное средство, противоправных действий третьих лиц, хищения транспортного средства.Страховое возмещение выплачивается полностью при наличии справки из ГАИ или милиции о невиновности арендатора, во всех других случаях удерживается франшиза в размере 200 - 400 USD, которую оплачивает Арендатор."));

                string contractTime = "\n6. СРОК ДЕЙСТВИЯ ДОГОВОРА ";
                body.AppendChild(CreateParagraph(contractTime, "\n"));
                body.AppendChild(CreateSingleParagraph($"6.1.Настоящий договор вступает в силу с {startDate} и действует по {endDate}"));
                body.AppendChild(CreateSingleParagraph("6.2.Арендатор вправе расторгнуть договор в любое время, письменно предупредив Арендодателя за 5(пять) дней до предполагаемой даты расторжения договора, в таком случае Арендатор возвращает автомобиль в порядке, определенном пунктом 2.9 настоящего договора, а Арендодатель обеспечивает перерасчет арендной платы при досрочном расторжении настоящего договора в соответствии с пунктом 4.3 настоящего договора."));

                string details = "\n7. АДРЕСА И РЕКВИЗИТЫ СТОРОН";
                body.AppendChild(CreateParagraph(details, "\n"));

                body.AppendChild(CreateParagraph("\nАРЕНДОДАТЕЛЬ", "\n"));

                body.AppendChild(CreateParagraph("Company: ", Name));
                body.AppendChild(CreateParagraph("Адрес: ", Address));
                body.AppendChild(CreateParagraph("Номер телефона: ", Phone));
                body.AppendChild(CreateParagraph("Электронный адрес: ", Email));
                body.AppendChild(CreateParagraph("Банк: ", BankName));
                body.AppendChild(CreateParagraph("Расчетный счет организации: ", AccountNumber));
                body.AppendChild(CreateParagraph("Коррекспондентский счет организации: ", CorrespondentAccount));
                body.AppendChild(CreateParagraph("БИК: ", BIK));
                body.AppendChild(CreateParagraph("ИНН: ", INN));

                body.AppendChild(CreateParagraph(" ", " ")); // Пустой абзац

                // Добавление информации о клиенте
                body.AppendChild(CreateParagraph("\nАРЕНДАТОР", "\n"));

                body.AppendChild(CreateParagraph("Имя: ", name));
                body.AppendChild(CreateParagraph("Фамилия: ", surname));
                body.AppendChild(CreateParagraph("Электронный адрес: ", email));
                body.AppendChild(CreateParagraph("Номер паспорта: ", passportNumber));
                body.AppendChild(CreateParagraph("Идентификационный номер: ", identificationNumber));
                body.AppendChild(CreateParagraph("Серия водительского удостоверения: ", licenseSeries));
                body.AppendChild(CreateParagraph("Номер водительского удостоверения: ", licenseNumber));

                // Сохранение изменений и закрытие документа
                document.Save();
            }
        }

        private Paragraph CreateParagraph(string label, string value)
        {
            // Создание абзаца с меткой и значением
            Paragraph paragraph = new Paragraph();

            // Создание стиля для метки
            RunProperties labelRunProperties = new RunProperties();
            Bold bold = new Bold();
            labelRunProperties.Append(bold);

            // Создание элемента Run для метки
            Run labelRun = new Run(new Text(label));
            labelRun.RunProperties = labelRunProperties;
            paragraph.AppendChild(labelRun);

            // Создание элемента Run для значения
            Run valueRun = new Run(new Text(value));
            paragraph.AppendChild(valueRun);

            return paragraph;
        }
        private Paragraph CreateSingleParagraph(string label)
        {
            // Создание абзаца с меткой и значением
            Paragraph paragraph = new Paragraph();

            // Создание стиля для метки
            RunProperties labelRunProperties = new RunProperties();
            Bold bold = new Bold();
            labelRunProperties.Append(bold);

            // Создание элемента Run для метки
            Run labelRun = new Run(new Text(label));
            labelRun.RunProperties = labelRunProperties;
            paragraph.AppendChild(labelRun);
            return paragraph;
        }
    }
}
