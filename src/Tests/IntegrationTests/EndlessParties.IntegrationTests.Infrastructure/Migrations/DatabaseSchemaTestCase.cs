namespace EndlessParties.IntegrationTests.Infrastructure.Migrations;

/// <summary>
/// Данные тестового случая для теста сравнения схемы базы данных
/// </summary>
public class DatabaseSchemaTestCase
{
    /// <summary>
    /// Наименование таблицы
    /// </summary>
    public required string TableName { get; init; }

    /// <summary>
    /// Столбцы
    /// </summary>
    public required IReadOnlyList<ColumnData> Columns { get; init; }

    /// <summary>
    /// Ограничения
    /// </summary>
    public required IReadOnlyList<ConstraintData> Constraints { get; init; }

    /// <summary>
    /// Внешние ключи
    /// </summary>
    public required IReadOnlyList<ForeignKeyData> ForeignKeys { get; init; }

    /// <summary>
    /// Индексы
    /// </summary>
    public required IReadOnlyList<IndexData> Indexes { get; init; }
}

/// <summary>
/// Ограничение
/// </summary>
public readonly struct ConstraintData
{
    /// <summary>
    /// Наименование
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Выражение
    /// </summary>
    public required string Expression { get; init; }
}

/// <summary>
/// Индекс
/// </summary>
public readonly struct IndexData
{
    /// <summary>
    /// Наименование
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Признак уникальности
    /// </summary>
    public required bool IsUnique { get; init; }

    /// <summary>
    /// Столбцы
    /// </summary>
    public required IReadOnlyList<ColumnData> Columns { get; init; }
}

/// <summary>
/// Внешний ключ
/// </summary>
public readonly struct ForeignKeyData
{
    /// <summary>
    /// Наименование
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Родительская таблица
    /// </summary>
    public required string RefersToTable { get; init; }

    /// <summary>
    /// Столбцы
    /// </summary>
    public required IReadOnlyList<string> Columns { get; init; }

    /// <summary>
    /// Правило при удалении
    /// </summary>
    public required string DeleteRule { get; init; }

    /// <summary>
    /// Правило при обновлении
    /// </summary>
    public required string UpdateRule { get; init; }
}

/// <summary>
/// Столбец
/// </summary>
public readonly struct ColumnData(string name)
{
    /// <summary>
    /// Наименование
    /// </summary>
    public readonly string Name = name;

    /// <summary>
    /// Оператор преобразования из строки
    /// </summary>
    public static implicit operator ColumnData(string value)
    {
        return new ColumnData(value);
    }
}