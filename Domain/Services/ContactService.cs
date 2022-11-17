using Domain.Commands;
using Domain.Commands.Contact;
using Domain.Commands.User;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys;
using Domain.Querys.Contact;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class ContactService
    {
        public IUserRoleRepository UserRoleRepository { get; set; }
        public IContactRepository ContactRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public ContactService(
            IUserRoleRepository userRoleRepository,
            IContactRepository contactRepository,
            IUserRepository userRepository)
        {
            UserRoleRepository = userRoleRepository;
            ContactRepository = contactRepository;
            UserRepository = userRepository;
        }
        public Task<PageData<ContactQuery>> FindAllContacts(PageQuery pageQuery)
        {
            return ContactRepository.FindAllContacts(pageQuery);
        }

        public AppContact FindContactById(long id)
        {
            return ContactRepository.Find(id) ?? throw new ValidateException(Messages.ContactNotFound);
        }

        public ImageQuery GetImageContact(long contactId)
        {
            return ContactRepository.FindImageContact(contactId) ?? throw new ValidateException(Messages.ImageNotFound);
        }

        public AppContact FindByDocumentNumber(string documentNumber)
        {
            return ContactRepository.FindByDocumentNumber(documentNumber);
        }

        public NameQuery FindLoginById(long id)
        {
            return UserRepository.FindLoginById(id);
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
            var contact = FindByDocumentNumber(command.DocumentNumber);

            if(contact != null)
                throw new ValidateException(Messages.AlreadyRegisteredUser);
            else
            {
                contact = ContactRepository.Add(new AppContact
                {
                    Name = command.Name,
                    SecondName = command.SecondName,
                    Gender = command.Gender,
                    DocumentNumber = command.DocumentNumber,
                    Birthdate = command.Birthdate,
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
            contact.Birthdate = command.Birthdate;
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

        public void UpdateImageContact(long id, ImageCommand command)
        {
            var contact = ContactRepository.Find(id) ?? throw new ValidateException(Messages.ContactNotFound);

            contact.ImageName = command.ImageName;
            contact.ImageUrl = command.ImageUrl;

            ContactRepository.Update(contact);
        }

        public void DeleteContact(long id)
        {
            var contact = ContactRepository.Find(id) ?? throw new ValidateException(Messages.ContactNotFound);
            var user = UserRepository.Find(id) ?? throw new ValidateException(Messages.UserNotFound);

            if(user != null)
            {
                var roles = UserRoleRepository.FindUserRoleByUserId(user.Id);

                foreach (var role in roles)
                    UserRoleRepository.Remove(role);

                UserRepository.Remove(user);
            }

            ContactRepository.Remove(contact);
        }
    }
}
