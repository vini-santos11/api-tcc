using Domain.Commands.Contact;
using Domain.Commands.User;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Models;

namespace Domain.Services
{
    public class ContactService
    {
        public IContactRepository ContactRepository { get; set; }
        public ContactService(IContactRepository contactRepository)
        {
            ContactRepository = contactRepository;
        }
        public AppContact FindByDocumentNumber(string documentNumber)
        {
            return ContactRepository.FindByDocumentNumber(documentNumber);
        }

        public ContactCommand RegisterCommandToContactCommand(RegisterCommand command)
        {
            var contactCommand = new ContactCommand
            {
                Name = command.Name,
                SecondName = command.SecondName,
                DocumentNumber = command.DocumentNumber,
                Gender = command.Gender,
                Birthdate = command.Birthdate,
                PersonType = command.PersonType,
                Phone = command.Phone,
                Email = command.Email
            };

            return contactCommand;
        }

        public AppContact CreateContactFromRegister(RegisterCommand command)
        {
            var contactCommand = RegisterCommandToContactCommand(command);
            var contact = CreateContact(contactCommand);

            return contact;
        }

        public AppContact UpdateContactFromRegister(long id, RegisterCommand command)
        {
            var contactCommand = RegisterCommandToContactCommand(command);
            var contact = UpdateContact(id, contactCommand);

            return contact;
        }

        public AppContact CreateContact(ContactCommand command)
        {
            var contact = FindByDocumentNumber(command.DocumentNumber) ?? throw new ValidateException(Messages.AlreadyRegisteredUser);
            
            if(contact == null)
            {
                contact = ContactRepository.Add(new AppContact
                {
                    Name = command.Name,
                    SecondName = command.SecondName,
                    Gender = command.Gender,
                    DocumentNumber = command.DocumentNumber,
                    Birthdate = command.Birthdate.GetValueOrDefault(),
                    Email = command.Email,
                    Address = command.Address,
                    Phone = command.Phone,
                    ImageName = command.ImageName,
                    ImageUrl = command.ImageUrl,
                    PersonTypeId = (int)command.PersonType
                });
            } 

            return contact;
        }

        public AppContact UpdateContact(long id, ContactCommand command)
        {
            var contact = ContactRepository.Find(id) ?? throw new ValidateException(Messages.ContactNotFound);

            contact.Name = command.Name;
            contact.SecondName = command.SecondName;
            contact.Address = command.Address;
            contact.Birthdate = command.Birthdate.GetValueOrDefault();
            contact.DocumentNumber = command.DocumentNumber;
            contact.Email = command.Email;
            contact.Gender = command.Gender;
            contact.ImageName = command.ImageName;
            contact.ImageUrl = command.ImageUrl;
            contact.Phone = command.Phone;
            contact.PersonTypeId = (int)command.PersonType;

            ContactRepository.Update(contact);

            return contact;
        }
    }
}
