// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;

namespace  Heiflow.AI.GeneticProgramming
{

    //Skup funkcija koje se mogu naci u GP ova klasa je popunjena iz XML datoteke
    [Serializable]
    public class GPFunction
    {
       
        //Osobine svake funkcije
        //Da li je odabrana uprogramu
        public bool Selected { get; set; }

        //Kolikoima argumenata
        public int Aritry { get; set; }

        //Naziv funkcije
        public string Name { get; set; }

        //Opis funkcije
        public string Description { get; set; }

        //Definicija funkcije po kojoj se izračunava
        public string Definition { get; set; }

        //Definicija funkcije u Excel notaciji
        public string ExcelDefinition { get; set; }

        //Da li je standardna ili definisana od strane korisnika
        public bool IsReadOnly { get; set; }

        //Da li je funkcija neka slozena funkcija koja zahtjeva 
        // dodatne parametre poput mean, deviation, rsquare ili slicno
        public bool IsDistribution { get; set; }

        //Relative proportionas of selecting function
        public int Weight { get; set; }

        //Function ID
        public ushort ID { get; set; }
        
        //TO DO: Future release will contains parameters
        //Parameters
        
        /// <summary>
        /// Override ToString member
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

    }
}
