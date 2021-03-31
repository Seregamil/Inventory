namespace API.Store
{
    public enum StorageType
    {
        /// <summary>
        /// Основной инвентарь игрока
        /// 1-25 - инвентарь
        /// 26-35 - сумка
        /// 101-104 - быстрые слоты
        /// 201-212 - экипировка
        /// </summary>
        Individual,
        
        /// <summary>
        /// Инвентарь для обмена
        /// </summary>
        Trade,
        
        /// <summary>
        /// Багажник
        /// </summary>
        Trunk,
        
        /// <summary>
        /// Шкаф
        /// </summary>
        Cupboard,
        
        /// <summary>
        /// Холодильник
        /// </summary>
        Fridge,
        
        /// <summary>
        /// Склад
        /// </summary>
        Stock,
    }
}