using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Verse;
using Verse.AI;

namespace Genoctome
{
    public class BaseGenoctome
    {
        public class specifications
        {
            //! Способность геноктома выполнять задачи :
            public float resistance = 0;    //! сопротивления урону и средам
            public float power = 0;         //! поддерживать и развивать физическую силу
            public float agility = 0;       //! поддерживать и развивать мобильность

            public float recovery = 0;      //! эффективность регенерации и иммунизации
            public float cleverness = 0;    //! развитие навыков

            public float manupulation = 0;  //! контороль колонии вне организма
        }
        public class modifyer
        {
            public float mod = 0;

            public float percent(float total)
            {
                return total / 100 * mod;
            }

            public double percent(double total)
            {
                return (double)percent((float)total);
            }

            public int percent(int total)
            {
                return (int)percent((float)total);
            }

        };
        public enum generation
        {
            PROTO, CIVIL, WORKING, MILITARY, EXPERIMENTAL
        }
        public enum serial
        {
            PROTISTA, PUROFAG, HOBGOBLIN
        }
        public abstract class perk
        {
            public string name;
            public bool available, ifSpliced;
            private bool established;

            public perk bestly = null, addicted = null;

            public bool getEstablished()
            {
                return established;
            }
            public void setEstablished(bool established, ref genoctome owner)
            {
                if (!this.established && established) enter(ref owner);
                this.established = established;
            }
            public virtual void enter(ref genoctome owner)
            {

            }
            public virtual void run(ref genoctome owner)
            {

            }
        }
        public class perkBook
        {

            List<perk> _container = new List<perk>();
            public perk selected;
            public genoctome owner;


            /// <summary>
            /// Добавляет в книгу навык, новый навык доступен в selected
            /// </summary>
            /// <param name="name">Название навыка</param>
            /// <param name="perk">Инструкции навыка</param>
            /// <param name="available">Доступно</param>
            /// <param name="established">Установлено</param>
            /// <param name="ifSpliced">Выполнять только при сращивании</param>
            public void add(string name, perk perk, bool available, bool established, bool ifSpliced)
            {
                _container.Add(perk);
                _container[_container.Count - 1].name = name;
                _container[_container.Count - 1].available = available;
                _container[_container.Count - 1].setEstablished(established, ref owner);
                _container[_container.Count - 1].ifSpliced = ifSpliced;

                selected = _container[_container.Count - 1];
            }
            public bool select(string name)
            {
                foreach (perk element in _container)
                {
                    if (element.name == name)
                    {
                        selected = element;
                        return true;
                    }
                }

                return false;
            }
            /// <summary>
            /// Сообщает selected-навыку другой лучший навык. Если лучший навык установлен, selected-навык не используется
            /// </summary>
            /// <param name="name">Имя лучшего навыка</param>
            public bool setBest(string name)
            {
                foreach(perk perk in _container){
                    
                    if (perk.name == name)
                    {
                        selected.bestly = perk;
                        return true;
                    }

                }

                return false;
            }
            /// <summary>
            /// Сообщает selected-навыку его зависимость от другого навыка. Если зависимость установлена, selected-навык не работает, пока не будет работать навык-источник зависимости.
            /// </summary>
            /// <param name="name">Имя навыка-источника зависимости</param>
            /// <returns></returns>
            public bool setAddict(string name)
            {

                foreach (perk perk in _container)
                {
                    if (perk.name == name)
                    {
                        selected.addicted = perk;
                        return false;
                    }
                }

                return false;
            }
            public void run()
            {
                if (owner == null)
                {
                    Log.Message("perkBook don't know owner");
                }  
                else
                    for (short i = 0; i < _container.Count(); i++)
                    {
                        bool bestly = false;
                        if (_container[i].bestly != null) bestly = _container[i].bestly.getEstablished();

                        bool addicted = true;
                        if (_container[i].addicted != null) addicted = _container[i].addicted.getEstablished();

                        //Навык исполняется только если : установлен, соблюден крит-ий сращивания, лучший навык не установлен, удовлетворена зависимость
                        if (
                            _container[i].getEstablished() &&
                            _container[i].ifSpliced == owner.ifSpliced() &&

                            !bestly && addicted
                           ) 
                            _container[i].run(ref owner);
                    }
            }
            public void report()
            {
                foreach (perk perk in _container)
                {
                    Log.Message(perk.name);
                    Log.Message("    available::" + perk.available.ToString() + ", established::" + perk.getEstablished().ToString() + ", ifSpliced::" + perk.ifSpliced.ToString());

                    if (perk.bestly != null)
                        Log.Message("        bestly::" + perk.bestly.name + ", set::" + perk.bestly.getEstablished().ToString());

                    if (perk.addicted != null)
                        Log.Message("        addicted::" + perk.addicted.name + ", set::" + perk.addicted.getEstablished().ToString());

                    Log.Message("");
                }
            }
        }
        public class genoctome
        {
            public static Random rnd;

            public int bestAge;

            public generation generation;               //! К какому поколению относится Геноктом  -2 2 6 10 14
              public modifyer gen;                      //! Модификатор влияния поколения
            public serial serial;                       //! К какой индивидуальной серии относится Геноктом
              public specifications specifications;     //!

            public Pawn Pawn;                           //! Пешка-носитель Геноктома
            public perkBook perkBook;                   //! Набор возможных навыков. Добавляйте навыки в HediffComp_Xxxx
            
            

            public genoctome()
            {
                rnd = new Random();
                bestAge = rnd.Next(19, 24);

                //! Конкретизация модификатора поколения
                gen = new modifyer();
                switch (generation)
                {
                    case generation.PROTO:
                        gen.mod = -0.02f;
                        break;

                    case generation.CIVIL:
                        gen.mod = 0.02f;
                        break;

                    case generation.WORKING:
                        gen.mod = 0.06f;
                        break;

                    case generation.MILITARY:
                        gen.mod = 0.1f;
                        break;

                    case generation.EXPERIMENTAL:
                        gen.mod = 0.14f;
                        break;
                }

                specifications = new specifications();
            }


            private void Init(Pawn Pawn)
            {
                Log.Message("  pawn in gen");
                this.Pawn = Pawn;

                Log.Message("  gen in perkBook");
                perkBook = new perkBook();
                perkBook.owner = this;
            }


            public void Insert(Pawn Pawn) 
            {
                Log.Message(" genoctom.init");
                Init(Pawn);

                Log.Message(" Splice");
                Pawn.health.AddHediff(HediffDefOfLocal.Splice);
            }
            public void inTick()
            {
                perkBook.run();
            }
            public void Remove()
            {
                if (ifSpliced()) Pawn.health.AddHediff(HediffDefOfLocal.PostAmputationInjury);
            }

            //*Добавляет-удаляет болезнь*//
            public void controlHediff(HediffDef Hediff, bool remove = false)
            {
                if (remove)
                {
                    Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(Hediff);
                    if (hediff != null) Pawn.health.RemoveHediff(hediff);

                }
                else Pawn.health.AddHediff(Hediff);
            }

            static public bool hasHediff(Pawn Pawn, HediffDef hediff)
            {
                List<Hediff> hediffs = Pawn.health.hediffSet.hediffs;

                for (short i = 0; i < hediffs.Count; i++)
                {
                    if (hediff.defName == hediffs[i].def.defName) return true;
                }

                return false;
            }

            static public bool hasHediff(Pawn Pawn, string tag)
            {
                List<Hediff> hediffs = Pawn.health.hediffSet.hediffs;

                for (short i = 0; i < hediffs.Count; i++)
                {
                    if (ifTag(tag, hediffs[i].def.tags)) return true;
                }

                return false;
            }

            static public BodyPartRecord takeBodyPart(Pawn pawn, string defName)
            {
                IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts();

                foreach(BodyPartRecord bodyPart in bodyParts)
                {
                    if (bodyPart.def.defName == defName) return bodyPart;
                }

                Log.Message($"takeBodyPart({defName}) return null");
                return null;
            }
            /// <summary>
            /// Копирует значения из одинаковых по имени полей от донора к реципиенту. Если донор и реципиент одинакового типа - все поля будут одинаковыми.
            /// </summary>
            /// <param name="donor">Объект-донор, откуда берутся значения.</param>
            /// <param name="recipient">Объект-рципиент, куда копируются значения.</param>
            /// <param name="deepCopy">Глубокое копирование, при котором, поля-объекты будут равны не ссылке на донорское поле, а инициализироваться и копировать данные из донорского поля.</param>
            public static void copyTo(object donor, object recipient, bool deepCopy = false)
            {
                FieldInfo[] fieldsDonor = donor.GetType().GetFields();
                FieldInfo[] fieldsRecip = recipient.GetType().GetFields();

                FieldInfo buffer;

                for (int i = 0; i < fieldsDonor.Length; i++)
                {

                    buffer = findByName(fieldsDonor[i], fieldsRecip);
                    if (buffer != null) buffer.SetValue(recipient, fieldsDonor[i].GetValue(donor));

                    if (deepCopy)
                    {
                        if (buffer.FieldType.IsClass)
                        {

                            ConstructorInfo[] constructors = buffer.FieldType.GetConstructors();

                            foreach (ConstructorInfo constructor in constructors)
                            {
                                if (constructor.GetParameters().Length == 0)
                                {
                                    buffer.SetValue(recipient, constructor.Invoke(null));
                                    copyTo(fieldsDonor[i].GetValue(donor), buffer.GetValue(recipient), deepCopy);
                                    break; //Console.WriteLine($"for {buffer.FieldType} not default constructor");
                                }
                            }
                        }
                    }
                }
            }
            public static FieldInfo findByName(FieldInfo field, FieldInfo[] fields, string report = null)
            {
                foreach (FieldInfo pointer in fields)
                {
                    if (field.Name == pointer.Name) return (pointer);
                }

                if (report != null)
                {
                    report = "findByName::not found";
                    if (field == null) report += " findByName::field == null";
                    if (fields == null || fields.Length == 0) report += " findByName::fields == null or fields.Leight() == 0";
                }
                return null;
            }

            static public Hediff_MissingPart takeMissingPart(Pawn pawn, string label)
            {
                List<Hediff_MissingPart> missingParts = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

                foreach (Hediff_MissingPart missingPart in missingParts)
                {
                    if (missingPart.def.label == label) return missingPart;
                }

                return null;
            }

            //*Проверяет завешён ли процесс сращивания*//
            public bool ifSpliced()
            {
                return !Pawn.health.hediffSet.HasHediff(HediffDefOfLocal.Splice);
            }

            //*Проверяет на естественность*//
            static public bool ifTag(string tag, List<string> tags)
            {
                //Если Лист не инициализиован, значит пуст - возвращаем false
                if (tags == null) return false;

                //Перебираем тэги на соответствие искомому
                for (short i = 0; i < tags.Count; i++)
                {
                    if (tags[i] == tag) return true;
                }

                //Если здесь, значит тэг не найден - false
                return false;
            }
            public BodyPartRecord getBodyPart(BodyPartDef bodyPart)
            {

                return null;
            }

            /*
            public void sd()
            {
                
            }

            public class JobDriver_ShowDF : JobDriver {

                public override bool TryMakePreToilReservations(bool errorOnFailed)
                {
                    Log.Message("Is it TryMakePreToilReservations");
                    return true;
                }

                protected override IEnumerable<Toil> MakeNewToils()
                {
                    Log.Message("Is it MakeNewToils");
                    if (job.targetA.Pawn == pawn && hasHediff(pawn, "Genoctome"))
                    {
                        Log.Message("    Check Target is true");
                        Toil hui = new Toil();

                        hui.actor = pawn;
                        hui.initAction = new 
                    }
                }

            }

            public class perkWindow : Window
            {
                public override void DoWindowContents(UnityEngine.Rect inRect)
                {

                }
            }*/

        }

    }
}
