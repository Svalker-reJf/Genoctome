using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Verse;

namespace Genoctome
{
    public class BaseGenoctome
    {
        public class speaker_withLog : reflection.speaker 
        {
            log logger = null;

            public speaker_withLog() { }
            public speaker_withLog(log logger)
            {
                this.logger = logger;
            }

            public override void speak(string text)
            {
                Log.Message(text);
                if (logger != null) logger.write(text);
            }
        }
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
            PROTISTA, PYROFAG, HOBGOBLIN
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

                for (short i = 0; i < hediffs.Count; ++i)
                {
                    if (hediff.defName == hediffs[i].def.defName) return true;
                }

                return false;
            }

            static public bool hasHediff(Pawn Pawn, string tag)
            {
                List<Hediff> hediffs = Pawn.health.hediffSet.hediffs;

                for (short i = 0; i < hediffs.Count; ++i)
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
            /// <summary>
            /// Выводит в консоль публичные поля объекта. 
            /// </summary>
            /// <param name="reporting">Рассматриваемый объект.</param>
            /// <param name="repeats">Количество отступов("  ").</param>
            public static void report(object reporting, MethodInfo speakerMethod = null, int repeats = 0)
            {

                FieldInfo[] fields = reporting.GetType().GetFields();

                for (int i = 0; i < fields.Length; ++i)
                {
                    if (fields[i].FieldType.IsValueType)
                        Console.WriteLine($"{getIndent(repeats)}{fields[i].FieldType.Name} {fields[i].Name} = {fields[i].GetValue(reporting)} ");

                    if (fields[i].FieldType.IsClass)
                    {
                        if (fields[i].GetValue(reporting) == null)
                            Console.WriteLine($"{getIndent(repeats)}{fields[i].FieldType.Name} {fields[i].Name} = null");

                        if (fields[i].FieldType.IsArray)
                        {
                            Array array = (Array)fields[i].GetValue(reporting);

                            Console.WriteLine();
                            Console.WriteLine($"{fields[i].FieldType.GetElementType()}[{array.Length}] {fields[i].Name}");
                            Console.WriteLine("[");

                            arrayContent(array, speakerMethod, repeats + 1);

                            Console.WriteLine("]");
                        }
                        else
                        if (fields[i].GetValue(reporting) != null)
                        {

                            Console.WriteLine();
                            Console.WriteLine($"{fields[i].FieldType.Name} {fields[i].Name}");
                            Console.WriteLine("{");

                            report(fields[i].GetValue(reporting), speakerMethod, repeats + 1);

                            Console.WriteLine("}");
                        }
                    }
                }
            }
            public static void callSpeaker(string speak)
            {
                Console.WriteLine(speak);
            }
            public static void arrayContent(Array array, MethodInfo speakerMethod, int repeats)
            {
                for (int a = 0; a < array.Length; ++a)
                {
                    var element = array.GetValue(a);
                    if (element.GetType().IsValueType)
                        Console.WriteLine($"{getIndent(repeats)}{a} {element.GetType().Name} = {element} ");
                    else report(array.GetValue(a), speakerMethod, repeats + 1);
                }
            }
            public static string getIndent(int repeats)
            {
                string indent = "";

                for (int i = repeats; i != 0; i--)
                {
                    indent += "  ";
                }

                return indent;
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
                for (short i = 0; i < tags.Count; ++i)
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
        public class reflection
        {
            /// <summary>
            /// Копирует значения из одинаковых по имени полей от донора к реципиенту. Если донор и реципиент одинакового типа - все поля будут одинаковыми.
            /// </summary>
            /// <param name="donor">Объект-донор, откуда берутся значения.</param>
            /// <param name="recipient">Объект-реципиент, куда копируются значения.</param>
            /// <param name="deepCopy">Глубокое копирование, при котором, поля-объекты будут равны не ссылке на донорское поле, а инициализироваться и копировать данные из донорского поля.</param>
            public static void copyTo<type>(ref type donor, ref type recipient, bool deepCopy = false, speaker speaker = null, bool copyPrivate = false, exeption exeption = null)
            {
                if (donor != null) 
                {
                    if (donor.GetType().IsValueType || donor.GetType().Name == "String")
                        copyValue(ref donor, ref recipient, speaker, copyPrivate, exeption);
                    else
                    if (donor.GetType().IsArray)
                        copyArray(ref donor, ref recipient, deepCopy, speaker, copyPrivate, exeption);
                    else
                    if (donor.GetType().IsClass)
                        copyClass(ref donor, ref recipient, deepCopy, speaker, copyPrivate, exeption);
                }
                else 
                    recipient = default(type);

            }
            static void copyValue<type>(ref type donor, ref type recipient, speaker speaker = null, bool copyPrivate = false, exeption exeption = null)
            {
                if (exeption != null && exeption.isExeption(recipient.GetType(), null))
                {
                    if (speaker != null) speaker.speak($"    {recipient.GetType()} is exeption");
                }
                else
                {
                    if (speaker != null)
                    {
                        speaker.speak($"    {recipient} = {donor}");
                        speaker.speak("");
                    }

                    recipient = donor;

                    if (!isEqual(recipient, donor) && speaker != null) speaker.speak($"    {recipient.GetType()} not copy {donor.GetType()}");
                }
            }
            static void copyArray<type>(ref type donor, ref type recipient, bool deepCopy = false, speaker speaker = null, bool copyPrivate = false, exeption exeption = null)
            {
                recipient = donor;
                
                if (deepCopy)
                {
                    if (exeption != null && exeption.isExeption(recipient.GetType(), null))
                    {
                        if (speaker != null) speaker.speak($"    {recipient.GetType()} is exeption");
                    }
                    else
                    {
                        if (donor != null)
                        {
                            if (speaker != null)
                                speaker.speak("[");

                            Array donorArray = (Array)(object)donor;

                            ConstructorInfo[] constructors = recipient.GetType().GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            foreach (ConstructorInfo constructor in constructors)
                            {
                                if (constructor.GetParameters().Length == 0)
                                {
                                    recipient = (type)constructor.Invoke(new object[] { donorArray.Length });
                                    break;
                                }
                            }

                            if (recipient != null)
                            {
                                Array recipArray = (Array)(object)recipient;

                                object shadow_d;
                                object shadow_r;

                                for (int i = 0; i < donorArray.Length; ++i)
                                {
                                    shadow_d = donorArray.GetValue(i);
                                    shadow_r = recipArray.GetValue(i);

                                    if (shadow_d != null && speaker != null)
                                        speaker.speak($"{shadow_d.GetType()}[{i}/{donorArray.Length - 1}]");

                                    copyTo(ref shadow_d, ref shadow_r, deepCopy, speaker, copyPrivate, exeption);

                                    recipArray.SetValue(shadow_r, i);
                                }

                                recipient = (type)(object)recipArray;
                                if (speaker != null)
                                    speaker.speak("");
                            }
                            else
                                if (speaker != null)
                                    speaker.speak($"    for {donor.GetType()} don't default constructor");

                            if (speaker != null)
                            {
                                speaker.speak("]");
                                speaker.speak("");
                            }
                        }
                        else
                            if (speaker != null)
                                speaker.speak($"    for {donor.GetType()} is null");
                    }
                }
            }
            static void copyClass<type>(ref type donor, ref type recipient, bool deepCopy = false, speaker speaker = null, bool copyPrivate = false, exeption exeption = null)
            {
                recipient = donor;

                if (deepCopy)
                {
                    if (exeption != null && exeption.isExeption(recipient.GetType(), null))
                    {
                        if (speaker != null) speaker.speak($"    {recipient.GetType()} is exeption");
                    }
                    else
                    {
                        if (donor != null)
                        {
                            ConstructorInfo[] constructors = recipient.GetType().GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            foreach (ConstructorInfo constructor in constructors)
                            {
                                if (constructor.GetParameters().Length == 0)
                                {
                                    recipient = (type)constructor.Invoke(null);
                                    break;
                                }
                            }
                            
                            if (recipient != null)
                            {
                                if (speaker != null)
                                    speaker.speak("{");

                                FieldInfo[] fieldsDonor = donor.GetType().GetFields(copyPrivate ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic : BindingFlags.Instance | BindingFlags.Public);
                                FieldInfo[] fieldsRecip = recipient.GetType().GetFields(copyPrivate ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic : BindingFlags.Instance | BindingFlags.Public);

                                for (int i = 0; i < fieldsDonor.Length; ++i)
                                {
                                    object shadow_d = fieldsDonor[i].GetValue(donor);
                                    object shadow_r = fieldsRecip[i].GetValue(recipient);
                                    Log.Message($"{recipient.GetType()}");
                                    if (exeption != null && exeption.isExeption(fieldsDonor[i].FieldType, fieldsDonor[i].Name))
                                    {
                                        if (speaker != null) speaker.speak($"    {fieldsDonor[i].FieldType} {fieldsDonor[i].Name} is exeption");
                                    }
                                    else
                                    {
                                        if (speaker != null)
                                            speaker.speak($"{fieldsDonor[i].Attributes} {fieldsDonor[i].FieldType} {fieldsDonor[i].Name}");

                                        if (shadow_d != null && shadow_r != null)
                                            copyTo(ref shadow_d, ref shadow_r, deepCopy, speaker, copyPrivate, exeption);

                                        fieldsRecip[i].SetValue(recipient, shadow_r);
                                    }
                                }

                                if (speaker != null)
                                {
                                    speaker.speak("}");
                                    speaker.speak("");
                                }
                            }
                            else
                                if (speaker != null)
                                    speaker.speak($"    for {donor.GetType().FullName} don't default constructor");
                        }
                        else
                            if (speaker != null)
                                speaker.speak($"    {donor.GetType().FullName} is null");

                    }
                }
            }


            /// <summary>
            /// Сравнивает два объекта на схожесть. Объекты должны быль одинакового типа.
            /// </summary>
            /// <param name="frst"></param>
            /// <param name="scnd"></param>
            /// <returns></returns>
            public static bool isEqual(object frst, object scnd)
            {
                if (frst != null)
                {
                    if (frst.GetType() == scnd.GetType())
                    {
                        if (frst.GetType().IsValueType)
                            return isValueType(frst, scnd);
                        else
                        if (frst.GetType().IsArray)
                            return isArray(frst, scnd);
                        else
                        if (frst.GetType().IsClass)
                            return isClass(frst, scnd);
                    }
                }
                else
                    if (frst == null && scnd == null) return true;

                return false;
            }
            static bool isValueType(object frst, object scnd)
            {
                if (!frst.Equals(scnd)) return false;

                return true;
            }
            static bool isArray(object frst, object scnd)
            {
                Array arrayFrst = (Array)frst;
                Array arrayScnd = (Array)scnd;

                if (arrayFrst == null && arrayScnd == null) return true;
                if (arrayFrst == null ^ arrayScnd == null) return false;
                if (arrayFrst.Length != arrayScnd.Length) return false;
                else
                {
                    for (int o = 0; o < arrayFrst.Length; ++o)
                    {
                        if (!isEqual(arrayFrst.GetValue(o), arrayScnd.GetValue(o))) return false;
                    }

                    return true;
                }
            }
            static bool isClass(object frst, object scnd)
            {

                if (frst == null && scnd == null) return true;

                FieldInfo[] infoFrst = frst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                FieldInfo[] infoScnd = scnd.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                for (int i = 0; i < infoFrst.Length; ++i)
                {
                    if (infoFrst[i].GetValue(frst) != null && infoScnd[i].GetValue(scnd) != null)
                        if (!isEqual(infoFrst[i].GetValue(frst), infoScnd[i].GetValue(scnd))) return false;
                }

                return true;
            }


            public static void reportEqual(object frst, object scnd, speaker speaker = null)
            {
                if (frst.GetType() == scnd.GetType())
                {
                    if (frst.GetType().IsValueType && !isValueType(frst, scnd))
                        reportValueType(frst, scnd, speaker);

                    if (frst.GetType().IsArray && !isArray(frst, scnd))
                        reportArray(frst, scnd, speaker);
                    else
                    if (frst.GetType().IsClass && !frst.GetType().IsArray && !isClass(frst, scnd))
                        reportClass(frst, scnd, speaker);
                }
            }
            static void reportValueType(object frst, object scnd, speaker speaker = null)
            {
                if (speaker != null)
                    speaker.speak($"  {frst} != {scnd}");
            }
            static void reportArray(object frst, object scnd, speaker speaker = null)
            {
                Array arrayFrst = (Array)frst;
                Array arrayScnd = (Array)scnd;


                if (arrayFrst.Length == arrayScnd.Length)
                {
                    for (int o = 0; o < arrayFrst.Length; ++o)
                    {
                        if (!isEqual(arrayFrst.GetValue(o), arrayScnd.GetValue(o)))
                        {
                            if (speaker != null)
                                speaker.speak($"  [{o}]");

                            reportEqual(arrayFrst.GetValue(o), arrayScnd.GetValue(o), speaker);
                        }
                    }
                }
                else
                    if (speaker != null)
                        speaker.speak($"  [{arrayFrst.Length}] and [{arrayScnd.Length}]");
            }
            static void reportClass(object frst, object scnd, speaker speaker = null)
            {

                FieldInfo[] infoFrst = frst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                FieldInfo[] infoScnd = scnd.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                for (int i = 0; i < infoFrst.Length; ++i)
                {
                    if (infoFrst[i].GetValue(frst) != null && infoScnd[i].GetValue(scnd) != null)
                        if (!isEqual(infoFrst[i].GetValue(frst), infoScnd[i].GetValue(scnd)))
                        {
                            if (speaker != null)
                                speaker.speak($"{infoFrst[i].FieldType.FullName} {infoFrst[i].Name}");
                            reportEqual(infoFrst[i].GetValue(frst), infoScnd[i].GetValue(scnd), speaker);
                        }
                }
            }

            public abstract class speaker
            {
                public virtual void speak(string text)
                {

                }
            }

            public class exeption
            {
                class _cell
                {
                    public string type = null;
                    public string name = null;
                }

                List<_cell> _container = new List<_cell>();

                public bool isExeption(Type type, string name)
                {
                    foreach (_cell buffer in _container)
                    {
                        if (buffer.type != null && buffer.name != null)
                            if (buffer.type == type.ToString() && buffer.name == name)
                                return true;

                        if (buffer.type != null && buffer.name == null)
                            if (buffer.type == type.ToString())
                                return true;

                        if (buffer.type == null && buffer.name != null)
                            if (buffer.name == name)
                                return true;
                    }

                    return false;
                }

                public void add(string type, string name)
                {
                    _cell buffer = new _cell();
                    buffer.type = type;
                    buffer.name = name;

                    _container.Add(buffer);
                }

            }
        }
        public class log
        {
            string path = @"C:\log.txt";
            StreamWriter writer;

            public log(string path)
            {
                this.path = path;

                writer = new StreamWriter(this.path, true);
                if (!File.Exists(this.path)) File.Create(this.path);

                writer.WriteLine($"/----{DateTime.Now}----/");
                writer.Flush();
            }
            public log()
            {
                writer = new StreamWriter(path, true);
                if (!File.Exists(path)) File.Create(path);

                writer.WriteLine($"/----{DateTime.Now}----/");
                writer.Flush();
            }

            public void write(string text)
            {
                writer.WriteLine(text);
                writer.Flush();
            }
        }
        ///<summary>
        ///Следит за привязанной к нему переменной. Не корректно работает с ссылочными типами данных.
        ///</summary>
        public class checker<type>
        {
            type _value = default(type);
            type _pastValue = default(type);

            public checker(ref type value)
            {
                _value = value;
            }
            public bool check()
            {
                bool buffer = false;

                if (_value.Equals(_pastValue)) buffer = true;

                _pastValue = _value;
                return buffer;
            }
        }

    }
}
