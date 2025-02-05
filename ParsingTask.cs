using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace linq_slideviews;

public class ParsingTask
{
	/// <param name="lines">все строки файла, которые нужно распарсить. Первая строка заголовочная.</param>
	/// <returns>Словарь: ключ — идентификатор слайда, значение — информация о слайде</returns>
	/// <remarks>Метод должен пропускать некорректные строки, игнорируя их</remarks>
	public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
	{
		return lines
			.Skip(1)
			.Select(x => x.Split(';'))
			.Where(x => 
				int.TryParse(x[0], out _) && 
				Enum.TryParse<SlideType>(x[1], true, out _))
			.Select(x => 
			{
				int slideId = int.Parse(x[0]);
				SlideType slideType = Enum.Parse<SlideType>(x[1], true);
				return new SlideRecord(slideId, slideType, x[2]);
			})
			.ToDictionary(x => x.SlideId, x => x);
	}

	/// <param name="lines">все строки файла, которые нужно распарсить. Первая строка — заголовочная.</param>
	/// <param name="slides">Словарь информации о слайдах по идентификатору слайда. 
	/// Такой словарь можно получить методом ParseSlideRecords</param>
	/// <returns>Список информации о посещениях</returns>
	/// <exception cref="FormatException">Если среди строк есть некорректные</exception>
	public static IEnumerable<VisitRecord> ParseVisitRecords(
		IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
	{
		return lines
			.Skip(1) // Пропускаем заголовок
			.Select(x => x.Split(';'))
			.Select(x =>
			{
				try
				{
					var userId = int.Parse(x[0]);
					var slideId = int.Parse(x[1]);
					
					var dateString = $"{x[2]} {x[3]}"; 
					var format = "yyyy-MM-dd HH:mm:ss";
					var dateTime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
					
					if (!slides.ContainsKey(slideId))
					{
						throw new FormatException($"Incorrect slideId : {slideId}");
					}
					var slideType = slides[slideId].SlideType;
					return new VisitRecord(userId, slideId, dateTime, slideType);
				}
				catch (FormatException ex)
				{
					throw new FormatException($"Wrong line [{string.Join(';', x)}]", ex);
				}
			})
			.ToList();
	}
}