using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NameGenerator
{
    static public string[] namesPool = {"Ahmed", "Mohamed", "Saif", "Sayyid", "Khalid", "Adil", "Mahmoud", "Jihad", "Abdullah", "Qais", "Ibrahim", "Mustafa", "Omer", "Musa", "Mutaz", 
                                "Abd Ellatif", "Abbas", "Suhaib", "Luay", "Hamad", "Zayid", "Jasim", "Taha", "El Amin", "Othman", "Abubakr", "Babiker", "Waddah", "Nasr Eldin",
                                "Suhail", "Amir", "Aamir", "Abd Elaziz", "Akram", "Ali", "Amjad", "Anas", "Asim", "Ayub", "Basil", "Dawood", "Ismail", "Fadi", "Faiz", "Fawaz",
                                "Islam", "Fakhri", "Fareed", "Faris", "Faruq", "Faisal", "Fikri", "Fakhri", "Fouad", "Gafar", "Ghassan", "Hamid", "Hadi", "Haidar", "Hakim",
                                "Hakeem", "Hamdi", "Hameed", "Hani", "Harith", "El-Harith", "Haroun", "Hasan", "Hassan", "Shihab", "Hashim", "Haydar", "Hisham", "Hussein",
                                "Hosni", "Muammar", "Husam", "Idris", "Ihab", "Imad", "Imran", "Irfan", "Ismat", "Jabir", "Jad", "Jamal", "El-Kamil", "Jameel", "Jawad", 
                                "Jibril", "Karam", "Khaleel", "Khalifa", "El-Khayyam", "Jubara", "Malik", "Magdi", "Wagdi", "Mahadi", "Mahir", "Mamoun", "Mansour", "Masud",
                                "Medhat", "Murad", "Nabil", "Adib", "Adham", "Sabri", "Nadir", "Hasanein", "Nagi", "El-Asam", "Najib", "Mahfouz", "Naseem", "Nasir", "Muntasir",
                                "Nazim", "Siraj", "Nizar", "Nour", "Omran", "Qasim", "Qusay", "Rashid", "Rajab", "El-Tayib", "Ramadan", "Ramiz", "Rasheed", "Rauf", "Riad",
                                "Ridwan", "Sabri", "Sabir", "Saddam", "Sadiq", "El-Sadiq", "Abdulrahman", "El-Safi", "Sajjad", "Rammah", "Salah", "Salim", "Samir", "Sameer",
                                "Sami", "Shadi", "Shahid", "Shakir", "Shamsuddin", "Shareef", "Sharfi", "Sulayman", "Sultan", "Tahir", "El-Tahir", "Tajuldin", "Waleed", 
                                "El-Waleed", "Waheeb", "Wail", "Wardi", "Kamal", "Yaqub", "Adam", "Yasir", "Yasin", "Yusuf", "Zahir", "Muzaffar", "Zuhair", "Zakaria", 
                                "Zaki", "El-Zaki", "Zain", "El-Zain", "Ziad", "Zulfaqar", "Abdulazim", "Abdulaziz", "Abdulsamad", "Abdulrahman", "Abdulrahim", "El-Qurashi"};

    static public Name GenerateRandomName()
    {
        Name name = new Name(namesPool[Random.Range(0, namesPool.Length)],
                            namesPool[Random.Range(0, namesPool.Length)]);
        return name;
    }
}

public struct Name
{
    public string first {get; private set;}
    public string last {get; private set;}

    public Name (string firstName, string lastName)
    {
        first = firstName;
        last = lastName;
    }
}