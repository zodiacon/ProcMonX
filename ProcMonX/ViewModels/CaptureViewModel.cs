using Prism.Mvvm;
using ProcMonX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {
	class CaptureViewModel : BindableBase {
		CaptureSettings _settings = new CaptureSettings();
		List<EventViewModel> _events;

		public CaptureViewModel() {
			_events = EventInfo.AllEvents.Select(evt =>
				new EventViewModel {
					Icon = $"/icons/events/{evt.EventType}.ico",
					IsSelected = _settings.EventTypes.Contains(evt.EventType),
					Name = evt.AsString,
					Category = evt.Category.ToString()
				}).ToList();

			SelectedCategory = Categories[0];
		}

		IList<CategoryViewModel> _categories = EventInfo.AllEventsByCategory.Select(cat =>
			new CategoryViewModel {
				Header = cat.Key.ToString(),
				Icon = $"/icons/category/{cat.Key.ToString()}.ico"
			}).ToList();

		public IList<CategoryViewModel> Categories => _categories;

		private CategoryViewModel _selectedCategory;

		public string Name {
			get => _settings.Name; 
			set {
				_settings.Name = value;
				RaisePropertyChanged(nameof(Name));
			}
		}

		public CategoryViewModel SelectedCategory {
			get => _selectedCategory;
			set {
				if (SetProperty(ref _selectedCategory, value)) {
					RaisePropertyChanged(nameof(EventTypes));
				}
			}
		}

		public IEnumerable<EventViewModel> EventTypes => _events.Where(evt => evt.Category == SelectedCategory?.Header).OrderBy(evt => evt.Name);
	}
}
