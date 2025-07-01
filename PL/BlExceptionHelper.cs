using System;
using System.Windows;
using BO;

namespace PL.Helpers
{
    public static class BlExceptionHelper
    {
        public static void ShowBlException(Exception ex)
        {
            switch (ex)
            {
                case BlAlreadyExistsException:
                case BlDoesNotExistException:
                case BlInvalidLogicException:
                case BlInvalidOperationException:
                case BlUnauthorizedAccessException:
                case BlApiRequestException:
                case BlInvalidFormatException:
                case BlGeolocationNotFoundException:
                case BlGeneralDatabaseException:
                case BlDeletionException:
                case BLTemporaryNotAvailableException:
                    MessageBox.Show(ex.Message, "Business Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;

                default:
                    MessageBox.Show($"Unexpected error:\n{ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
    }
}
