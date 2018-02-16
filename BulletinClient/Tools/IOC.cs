using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.DCT;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.Tools
{
    /// <summary>
    /// Ioc контейнер:
    /// 1. Фабрика объектов
    /// 2. Повторное использование объектов
    /// 3. Прединициализация объектов - 
    /// 4. Фоновация инициализация по ассембли проекта
    /// 
    /// Сделать базовый контейнер
    /// Сделать базовый объект или интерфейс
    /// </summary>
    class IOC<TBase>
    {
        #region Property
        public IEnumerable<TBase> Value { get { return instances.ToArray(); } }
        /// <summary>
        /// Объект синхронизации доступа к списку инстанций
        /// </summary>
        private readonly List<TBase> instances = new List<TBase>();
        /// <summary>
        /// Создаёт объект в проекте, из которого вызывается
        /// </summary>
        public Func<string, TBase> CreateInstance { get; set; }

        #endregion
        #region Methods
        public T GetInstance<T>()
        {
            return GetInstance<T>(typeof(T).ToString());
        }
        static object addObj = new object();
        public T GetInstance<T>(string type)
        {
            var result = default(T);
            DCT.Execute(data =>
            {
                //1. Ищем объект
                var r = instances.OfType<T>().Where(q => q.GetType().FullName == type);
                var rf = r.FirstOrDefault();
                if (rf != null && rf is T)
                {
                    result = rf;
                    if (r.Count() > 1)
                        throw new Exception("Более одного экземпляра класса в списке инстанций");
                    return;
                }
                //2. Создаём объект
                object r2 = null;

                if (CreateInstance != null)
                    r2 = CreateInstance(type);
                else
                    r2 = Assembly.GetExecutingAssembly().CreateInstance(type);
                if (r2 != null && r2 is T && r2 is TBase)
                    Add((TBase)r2);
                else
                    throw new Exception("Не корректный тип объекта или его не удалось создать тк Assembly не доступно");
                result = GetInstance<T>(type);
            });
            return result;
        }
        public object GetInstance(Type type)
        {
            object result = null;
            DCT.Execute(data =>
            {
                //1. Ищем объект
                var r = instances.Where(q => q.GetType() == type);
                var rf = r.FirstOrDefault();
                if (rf != null && rf.GetType() == type)
                {
                    result = rf;
                    if (r.Count() > 1)
                        throw new Exception("Более одного экземпляра класса в списке инстанций");
                    return;
                }

                //2. Создаём объект
                object r2 = null;

                if (CreateInstance != null)
                    r2 = CreateInstance(type.ToString());
                else
                    r2 = Assembly.GetExecutingAssembly().CreateInstance(type.ToString());
                if (r2 != null && r2.GetType() == type && r2 is TBase)
                    Add((TBase)r2);
                else
                    throw new Exception("Не корректный тип объекта или его не удалось создать тк Assembly не доступно");
                result = GetInstance(type);
            });
            return result;
        }
        /// <summary>
        /// Выполняет дейсвие для указанной формы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public void Execute<T>(Action<T> expression)
        {
            DCT.Execute(data =>
            {
                if (expression == null)
                    throw new NullReferenceException("expression не может быть пустым");
                var obj = GetInstance<T>();
                expression(obj);
            });
        }
        public void SetContent<T>(Expression<Func<T, object>> memberLamda, Type type)
        {
            DCT.Execute(data =>
            {
                var content = GetInstance<T>();
                var value = GetInstance(type);
                SetContent(content, memberLamda, value);
            });
        }
        /// <summary>
        /// Присваивает указанному свойству, инстанцию объекта 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        public void SetContent<T, TValue>(T target, Expression<Func<T, TValue>> expression, TValue obj)
        {
            DCT.Execute(data =>
            {
                Execute<T>(view =>
                {
                    var name = SetPropertyValue(view, expression, obj);
                    if (view is NotifyObject)
                    {
                        var r = view as NotifyObject;
                        r.RaisePropertyChanged(name);
                    }
                });
            });
        }
        private static string SetPropertyValue<T, TValue>(T target, Expression<Func<T, TValue>> memberLamda, TValue value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value, null);
                    return property.Name;
                }
            }
            return "";
        }
        #endregion
        public void Add(TBase dc)
        {
            DCT.Execute(data =>
            {
                if (dc != null)
                {
                    lock (addObj)
                    {
                        if (instances.OfType<TBase>().Any(q => q.GetType().FullName == typeof(TBase).ToString()))
                        {
                            ObjectHelper.Dispose(dc);
                            return;
                        }
                        var type = dc.GetType();
                        if (!instances.Any(q => q.GetType() == type))
                            instances.Add(dc);
                    }
                }
            });
        }
        public void Remove(Type type)
        {
            DCT.Execute(data =>
            {
                var r = instances.SingleOrDefault(q => q.GetType() == type);
                if (r != null)
                {
                    instances.Remove(r);
                    ObjectHelper.Dispose(r);
                }
            });
        }
        /// <summary>
        /// Сделать список по игнору моделей - главное окно, основной контент и всплывающие окна. Все другие модели очищать через Dispose
        /// </summary>
        public void Clear()
        {
            DCT.Execute(data =>
            {
                foreach (var item in instances)
                {

                }
            });
        }


    }
}
