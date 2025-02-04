using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public static class ExtensionsTask
{
	/// <summary>
	/// Медиана списка из нечетного количества элементов — это серединный элемент списка после сортировки.
	/// Медиана списка из четного количества элементов — это среднее арифметическое 
    /// двух серединных элементов списка после сортировки.
	/// </summary>
	/// <exception cref="InvalidOperationException">Если последовательность не содержит элементов</exception>
	public static double Median(this IEnumerable<double> items)
	{
		if (!items.Any())
			throw new InvalidOperationException();

		return items.Count() % 2 != 0
			? items
				.OrderBy(x => x)
				.Skip(items.Count() / 2)
				.First()
			: items
				.OrderBy(x => x)
				.Skip(items.Count() / 2 - 1)
				.Take(2)
				.Average();
	}

	/// <returns>
	/// Возвращает последовательность, состоящую из пар соседних элементов.
	/// Например, по последовательности {1,2,3} метод должен вернуть две пары: (1,2) и (2,3).
	/// Sequence should be enumerated lazily!
	/// </returns>
	public static IEnumerable<(T First, T Second)> Bigrams<T>(this IEnumerable<T> items)
	{
		var arrayItems = items.ToArray();
		return arrayItems
			.Take(arrayItems.Length - 1)
			.Select(x => (x, arrayItems[Array.IndexOf(arrayItems, x) + 1]));
	}
}