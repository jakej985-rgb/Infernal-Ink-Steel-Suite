#include "appointmentdialog.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QMessageBox>
#include <QGraphicsDropShadowEffect>
#include <QPropertyAnimation>
#include <QEasingCurve>

AppointmentDialog::AppointmentDialog(QSqlDatabase* db, int currentUserId, bool isAdmin, QWidget *parent)
    : ThemeableDialog(parent), m_db(db), m_currentUserId(currentUserId), m_isAdmin(isAdmin)
{
    setWindowTitle("Schedule Appointment");
    setFixedSize(400, 300);
    setWindowFlags(Qt::FramelessWindowHint | Qt::Dialog);
    setWindowOpacity(0.0);

    // Glow effect
    QGraphicsDropShadowEffect *glow = new QGraphicsDropShadowEffect(this);
    glow->setBlurRadius(30);
    glow->setOffset(0, 0);
    glow->setColor(QColor(0, 170, 255, 80));
    setGraphicsEffect(glow);

    setupUi();
    loadClients();

    // Fade in animation
    QPropertyAnimation *fade = new QPropertyAnimation(this, "windowOpacity");
    fade->setDuration(250);
    fade->setStartValue(0.0);
    fade->setEndValue(1.0);
    fade->start(QAbstractAnimation::DeleteWhenStopped);
}

void AppointmentDialog::setupUi()
{
    clientComboBox = new QComboBox(this);
    dateTimeEdit = new QDateTimeEdit(QDateTime::currentDateTime(), this);
    notesEdit = new QPlainTextEdit(this);

    saveButton = new QPushButton("Save", this);
    cancelButton = new QPushButton("Cancel", this);
    addClientButton = new QPushButton("Add Client", this);

    // Button glow effects
    // GlowEffect *glowSave = new GlowEffect(saveButton, this);
    // GlowEffect *glowCancel = new GlowEffect(cancelButton, this);
    // GlowEffect *glowAdd = new GlowEffect(addClientButton, this);

    QVBoxLayout *mainLayout = new QVBoxLayout(this);
    mainLayout->addWidget(clientComboBox);
    mainLayout->addWidget(dateTimeEdit);
    mainLayout->addWidget(notesEdit);

    QHBoxLayout *btnLayout = new QHBoxLayout();
    btnLayout->addWidget(addClientButton);
    btnLayout->addWidget(cancelButton);
    btnLayout->addWidget(saveButton);
    mainLayout->addLayout(btnLayout);

    setLayout(mainLayout);

    connect(saveButton, &QPushButton::clicked, this, &AppointmentDialog::saveAppointment);
    connect(cancelButton, &QPushButton::clicked, this, &AppointmentDialog::reject);
    connect(addClientButton, &QPushButton::clicked, this, &AppointmentDialog::openAddClientDialog);
}

void AppointmentDialog::loadClients()
{
    clientComboBox->clear();
    ClientDB clientDb(m_db);

    QVector<Client> clients = clientDb.getAllClients();
    for (const Client &client : clients) {
        QString fullName = client.firstName;
        if (!client.middleName.isEmpty()) fullName += " " + client.middleName;
        if (!client.lastName.isEmpty()) fullName += " " + client.lastName;

        clientComboBox->addItem(fullName, client.id);
    }
}

void AppointmentDialog::setAppointment(const Appointment &appt)
{
    m_appointment = appt;

    int index = clientComboBox->findData(appt.clientId);
    if (index >= 0)
        clientComboBox->setCurrentIndex(index);

    dateTimeEdit->setDateTime(appt.dateTime);
    notesEdit->setPlainText(appt.notes);
}

void AppointmentDialog::saveAppointment()
{
    if (dateTimeEdit->dateTime() < QDateTime::currentDateTime()) {
        QMessageBox::warning(this, "Invalid Date", "Cannot schedule an appointment in the past.");
        return;
    }

    int clientId = clientComboBox->currentData().toInt();
    m_appointment.clientId = clientId;
    m_appointment.userId = m_currentUserId;
    m_appointment.clientName = clientComboBox->currentText();
    m_appointment.dateTime = dateTimeEdit->dateTime();
    m_appointment.notes = notesEdit->toPlainText();

    AppointmentDB apptDb(m_db);

    if (m_appointment.id == 0) {
        if (!apptDb.addAppointment(m_appointment)) {
            QMessageBox::warning(this, "Error", "Failed to add appointment. Please check the details and try again.");
            return; // Stay on the dialog
        }
    } else {
        if (!apptDb.updateAppointment(m_appointment)) {
            QMessageBox::warning(this, "Error", "Failed to update appointment.");
            return; // Stay on the dialog
        }
    }

    accept();
}

void AppointmentDialog::openAddClientDialog()
{
    ClientDialog dlg(this);
    if (dlg.exec() == QDialog::Accepted) {
        Client newClient = dlg.getClient();

        ClientDB clientDb(m_db);
        int newId = clientDb.addClient(newClient); // returns new client ID

        if (newId > 0) {
            loadClients();
            int index = clientComboBox->findData(newId);
            if (index >= 0) clientComboBox->setCurrentIndex(index);
        } else {
            QMessageBox::warning(this, "Error", "Failed to add new client to the database.");
        }
    }
}