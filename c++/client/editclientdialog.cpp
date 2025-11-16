#include "editclientdialog.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QMouseEvent>
#include <QMessageBox>
#include <QEasingCurve>

EditClientDialog::EditClientDialog(QSqlDatabase* db, QWidget* parent)
    : ThemeableDialog(parent), m_db(db), mousePressed(false)
{
    setWindowFlags(Qt::FramelessWindowHint | Qt::Dialog);
    setWindowOpacity(0.0);
    setFixedSize(420, 380);
    setWindowTitle("Edit Client");

    setupUi();
    fadeIn();
}

void EditClientDialog::setupUi()
{
    auto* layout = new QVBoxLayout(this);
    layout->setContentsMargins(20, 20, 20, 20);
    layout->setSpacing(10);

    auto* title = new QLabel("Edit Client");
    title->setAlignment(Qt::AlignCenter);
    title->setObjectName("h2"); // For styling from QSS
    layout->addWidget(title);

    firstNameEdit = new QLineEdit(this); firstNameEdit->setPlaceholderText("First Name");
    middleNameEdit = new QLineEdit(this); middleNameEdit->setPlaceholderText("Middle Name");
    lastNameEdit = new QLineEdit(this); lastNameEdit->setPlaceholderText("Last Name");
    phoneEdit = new QLineEdit(this); phoneEdit->setPlaceholderText("Phone");
    emailEdit = new QLineEdit(this); emailEdit->setPlaceholderText("Email");

    layout->addWidget(firstNameEdit);
    layout->addWidget(middleNameEdit);
    layout->addWidget(lastNameEdit);
    layout->addWidget(phoneEdit);
    layout->addWidget(emailEdit);

    auto* btnLayout = new QHBoxLayout();
    cancelButton = new QPushButton("Cancel", this);
    saveButton = new QPushButton("Save Changes", this);
    btnLayout->addWidget(cancelButton);
    btnLayout->addWidget(saveButton);
    layout->addLayout(btnLayout);

    connect(saveButton, &QPushButton::clicked, this, &EditClientDialog::saveClient);
    connect(cancelButton, &QPushButton::clicked, this, &EditClientDialog::reject);

    glowEffect = new QGraphicsDropShadowEffect(this);
    glowEffect->setBlurRadius(30);
    glowEffect->setColor(QColor(0, 170, 255, 100));
    glowEffect->setOffset(0, 0);
    setGraphicsEffect(glowEffect);
}

void EditClientDialog::setClient(const Client& client)
{
    m_client = client;
    firstNameEdit->setText(client.firstName);
    middleNameEdit->setText(client.middleName);
    lastNameEdit->setText(client.lastName);
    phoneEdit->setText(client.phone);
    emailEdit->setText(client.email);
}

Client EditClientDialog::getClient() const
{
    Client updated = m_client;
    updated.firstName = firstNameEdit->text();
    updated.middleName = middleNameEdit->text();
    updated.lastName = lastNameEdit->text();
    updated.phone = phoneEdit->text();
    updated.email = emailEdit->text();
    return updated;
}

void EditClientDialog::saveClient()
{
    m_client = getClient();

    if (m_client.firstName.isEmpty() || m_client.lastName.isEmpty()) {
        QMessageBox::warning(this, "Missing Info", "First and last names are required.");
        return;
    }

    ClientDB clientDb(m_db);
    if (m_client.id == -1) {
        if (!clientDb.addClient(m_client))
            QMessageBox::warning(this, "Error", "Failed to add new client.");
    } else {
        if (!clientDb.updateClient(m_client))
            QMessageBox::warning(this, "Error", "Failed to update client.");
    }

    accept();
}

void EditClientDialog::fadeIn()
{
    fadeAnimation = new QPropertyAnimation(this, "windowOpacity");
    fadeAnimation->setDuration(250);
    fadeAnimation->setStartValue(0.0);
    fadeAnimation->setEndValue(1.0);
    fadeAnimation->start(QAbstractAnimation::DeleteWhenStopped);
}

void EditClientDialog::fadeOut()
{
    fadeAnimation = new QPropertyAnimation(this, "windowOpacity");
    fadeAnimation->setDuration(200);
    fadeAnimation->setStartValue(1.0);
    fadeAnimation->setEndValue(0.0);
    connect(fadeAnimation, &QPropertyAnimation::finished, this, &QDialog::close);
    fadeAnimation->start(QAbstractAnimation::DeleteWhenStopped);
}

// --- Drag behavior ---
void EditClientDialog::mousePressEvent(QMouseEvent* event)
{
    if (event->button() == Qt::LeftButton) {
        mousePressed = true;
        mousePressPos = event->globalPosition().toPoint() - frameGeometry().topLeft();
    }
}
void EditClientDialog::mouseMoveEvent(QMouseEvent* event)
{
    if (mousePressed)
        move(event->globalPosition().toPoint() - mousePressPos);
}
void EditClientDialog::mouseReleaseEvent(QMouseEvent*)
{
    mousePressed = false;
}

void EditClientDialog::accept()
{
    fadeOut();
    QDialog::accept();
}

void EditClientDialog::reject()
{
    fadeOut();
    QDialog::reject();
}
