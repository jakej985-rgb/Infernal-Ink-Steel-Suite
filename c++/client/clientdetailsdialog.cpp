#include "clientdetailsdialog.h"
#include "db/appointmentdb.h"
#include "domain/appointment.h"
#include "helper/thememanager.h"

#include <QVBoxLayout>
#include <QGroupBox>
#include <QTableWidgetItem>
#include <QPushButton>
#include <QHeaderView>
#include <QAbstractItemView>
#include <QGraphicsDropShadowEffect>
#include <QTimer>
#include <algorithm>
#include <QVector>

namespace {
    bool isMeaningfulVisit(const QString &status)
    {
        const QString normalized = status.trimmed().toLower();
        return !(normalized == "canceled" ||
                 normalized == "cancelled" ||
                 normalized == "no show" ||
                 normalized == "noshow");
    }
}

ClientDetailsDialog::ClientDetailsDialog(int clientId,
                                         const QString &firstName,
                                         const QString &middleName,
                                         const QString &lastName,
                                         const QString &phone,
                                         const QString &email,
                                         int visits,
                                         AppointmentDB *appointmentDb,
                                         QWidget *parent)
    : ThemeableDialog(parent),
      m_clientId(clientId),
      m_initialVisits(visits),
      m_appointmentDb(appointmentDb)
{
    setFixedSize(500, 380);
    fadeIn();

    QString fullName = firstName + (middleName.isEmpty() ? "" : " " + middleName) + " " + lastName;

    // Info Box
    QGroupBox *infoBox = new QGroupBox("Client Info");

    idLabel     = new QLabel("Client ID: " + QString::number(clientId));
    nameLabel   = new QLabel("Name: " + fullName);
    emailLabel  = new QLabel("Email: " + email);
    phoneLabel  = new QLabel("Phone: " + phone);
    visitsLabel = new QLabel("Visits: " + QString::number(visits));

    QVBoxLayout *infoLayout = new QVBoxLayout;
    infoLayout->addWidget(idLabel);
    infoLayout->addWidget(nameLabel);
    infoLayout->addWidget(emailLabel);
    infoLayout->addWidget(phoneLabel);
    infoLayout->addWidget(visitsLabel);
    infoBox->setLayout(infoLayout);

    // History Table
    historyTable = new QTableWidget(0, 2, this);
    historyTable->setHorizontalHeaderLabels({"Date", "Notes"});
    historyTable->horizontalHeader()->setStretchLastSection(true);
    historyTable->setAlternatingRowColors(true);
    historyTable->setSelectionMode(QAbstractItemView::NoSelection);
    historyTable->setEditTriggers(QAbstractItemView::NoEditTriggers);
    historyTable->setWordWrap(true);

    populateHistory();

    // Close Button
    closeButton = new QPushButton("Close");
    closeButton->setFixedHeight(36);
    connect(closeButton, &QPushButton::clicked, this, &ThemeableDialog::reject);

    QHBoxLayout *btnLayout = new QHBoxLayout;
    btnLayout->addStretch();
    btnLayout->addWidget(closeButton);

    // Main Layout
    QVBoxLayout *mainLayout = new QVBoxLayout(this);
    mainLayout->addWidget(infoBox);
    mainLayout->addWidget(historyTable);
    mainLayout->addLayout(btnLayout);
    setLayout(mainLayout);

    setWindowTitle("Client Details - " + fullName);
}

void ClientDetailsDialog::populateHistory()
{
    historyTable->setRowCount(0);
    historyTable->clearSpans();

    if (!m_appointmentDb) {
        showEmptyHistoryPlaceholder();
        return;
    }

    QVector<Appointment> appointments = m_appointmentDb->getAppointmentsForClient(m_clientId);
    if (appointments.isEmpty()) {
        showEmptyHistoryPlaceholder();
        visitsLabel->setText("Visits: " + QString::number(m_initialVisits));
        return;
    }

    std::sort(appointments.begin(), appointments.end(), [](const Appointment &a, const Appointment &b) {
        return a.dateTime > b.dateTime;
    });

    int countedVisits = 0;

    int row = 0;
    for (const Appointment &appt : appointments) {
        historyTable->insertRow(row);

        QTableWidgetItem *dateItem = new QTableWidgetItem(formatDateWithStatus(appt));
        dateItem->setFlags(dateItem->flags() & ~Qt::ItemIsEditable);
        historyTable->setItem(row, 0, dateItem);

        QString notesText = appt.notes.trimmed();
        if (notesText.isEmpty())
            notesText = QStringLiteral("(No notes recorded)");

        QTableWidgetItem *noteItem = new QTableWidgetItem(notesText);
        noteItem->setFlags(noteItem->flags() & ~Qt::ItemIsEditable);
        historyTable->setItem(row, 1, noteItem);

        if (isMeaningfulVisit(appt.status))
            ++countedVisits;

        ++row;
    }

    if (countedVisits > 0)
        visitsLabel->setText("Visits: " + QString::number(countedVisits));
    else
        visitsLabel->setText("Visits: " + QString::number(m_initialVisits));
}

QString ClientDetailsDialog::formatDateWithStatus(const Appointment &appt) const
{
    if (!appt.dateTime.isValid())
        return QStringLiteral("Unknown");

    const QString dateText = appt.dateTime.toString("yyyy-MM-dd hh:mm");
    const QString statusText = appt.status.isEmpty() ? QStringLiteral("Scheduled") : appt.status;
    return QStringLiteral("%1 (%2)").arg(dateText, statusText);
}

void ClientDetailsDialog::showEmptyHistoryPlaceholder()
{
    historyTable->setRowCount(1);
    historyTable->setSpan(0, 0, 1, 2);

    QTableWidgetItem *item = new QTableWidgetItem(QStringLiteral("No visit history recorded yet."));
    item->setTextAlignment(Qt::AlignCenter);
    item->setFlags(Qt::NoItemFlags);
    historyTable->setItem(0, 0, item);
}

void ClientDetailsDialog::updateTheme() {
    const Theme theme = ThemeManager::instance()->currentTheme();
    setStyleSheet(QString("background-color:%1; border-radius:12px;").arg(theme.background.name()));
    setFont(theme.font);

    QGroupBox *infoBox = findChild<QGroupBox *>();
    if (infoBox) {
        infoBox->setStyleSheet(QString(R"(
            QGroupBox {
                color: %1;
                font-weight: bold;
                border: 2px solid %2;
                border-radius: 12px;
                margin-top: 6px;
                padding: 8px;
                background-color: %3;
            }
        )")
                                   .arg(theme.accent.name(),
                                        theme.primary.name(),
                                        theme.background.darker(110).name()));
    }

    for (auto lbl : {idLabel, nameLabel, emailLabel, phoneLabel, visitsLabel}) {
        lbl->setStyleSheet(QString("color: %1; font-size: 14px;").arg(theme.text.name()));
    }

    historyTable->setStyleSheet(QString(R"(
        QTableWidget {
            background-color: %1;
            color: %2;
            border: 2px solid %3;
            border-radius: 8px;
            gridline-color: %3;
        }
        QHeaderView::section {
            background-color: %4;
            color: %5;
            font-weight: bold;
            padding: 4px;
            border: 1px solid %3;
        }
        QTableWidget::item:selected {
            background-color: %3;
            color: black;
        }
        QTableWidget::item:hover {
            border: 1px solid %5;
        }
    )")
                                    .arg(theme.background.name(),
                                         theme.text.name(),
                                         theme.primary.name(),
                                         theme.primary.darker(150).name(),
                                         theme.accent.name()));

    closeButton->setStyleSheet(QString(R"(
        QPushButton {
            background-color: %1;
            color: %2;
            border-radius: 10px;
            font-weight: bold;
        }
        QPushButton:hover {
            background-color: %3;
            color: black;
        }
    )")
                                   .arg(theme.primary.name(),
                                        theme.text.name(),
                                         theme.accent.name()));
    applyGlow(closeButton, QColor(0, 255, 255, 120));
}
